import collections
import itertools
import numpy as np
from operator import itemgetter
from scipy.spatial import ConvexHull
from scipy.spatial import distance
import re

DEBUG = False

def log(message):
    """Prints a message only if DEBUG = True, so that all printing to stdout can be easily disabled."""
    if DEBUG:
        print(message)

def are_same(reference, value, percentage):
    min_value = reference - reference * percentage
    max_value = reference + reference * percentage
    result = min_value < value < max_value
    
    return result

def string_to_coords(coord_string):
    """
    Checks and decodes a coordinates string (that is passed to the API on the command line) into coordinates.
    Returns an empty list if it is not well formed.
    """
    if not isinstance(coord_string, str):
        return []
    
    coord_string = re.sub(r'\s+', '', coord_string, flags=re.UNICODE)
    is_well_formed = re.match(r'\[(\(\d+,\d+\),){0,}(\(\d+,\d+\))\]', coord_string)
    
    return eval(coord_string) if is_well_formed else []


def approximates(ref_point, point, max_deviation):
    """Helper function to check if two points are the same within the specified deviation."""
    
    x = ref_point[0] - max_deviation <= point[0] <= ref_point[0] + max_deviation
    y = ref_point[1] - max_deviation <= point[1] <= ref_point[1] + max_deviation
    
    return x and y


def get_orientation_marks(points):
    """
    Extract the reference system (o, vx, vy) from a set of points. 
    
    Returns (None, None, None) if no reference system found.
    """
    p_threshold = 0.08
    no_result = None

    # no touchcode if there are not enough points
    if points is None or len(points) < 3:
        return no_result

    # create a convex hull with all points
    try:
        hull = ConvexHull(points)  
    except:
        return no_result
    
    vectors = []
    
    # calculate all possible distances between all points
    for combination in list(itertools.combinations(hull.vertices, 2)):
        p1 = points[combination[0]]
        p2 = points[combination[1]]
        dst = distance.euclidean(p1, p2)
        
        vectors.append((p1, p2, dst))      
        
    # get the two points that have the longest distance (those are vx and vy)
    orientation = max(vectors, key=itemgetter(2))
    v1 = orientation[0]
    v2 = orientation[1]
    
    log("v1: {0}, v2: {1}, dst(v1, v2): {2}]".format(v1, v2, orientation[2]))
    
    # filter distances list to not contain the longest distance anymore
    vectors = [a for a in vectors if not orientation[2] == a[2]]
    
    origin = None
    candidates = []
    
    # find the origin candidates by getting all distances that are longest_distance / sqrt(2)    
    for vector in vectors:
        if are_same(orientation[2] / np.sqrt(2), vector[2], p_threshold):
            if np.array_equal(vector[0], v1) or np.array_equal(vector[0], v2):
                candidates.append((vector[1][0], vector[1][1]))
            if np.array_equal(vector[1], v1) or np.array_equal(vector[1], v2):
                candidates.append((vector[0][0], vector[0][1]))
    
    # find the origin (the point that we got twice)
    try:
        origin = np.array([k for k, v in collections.Counter(candidates).items() if v == 2])[0]
    except:
        return no_result
    
    log("origin: {0}".format(origin))
    
    vx, vy = find_vx_vy(hull, origin, v1, v2)
    
    log("vx: {0}, vy: {1}".format(vx, vy))
    
    return (origin, vx, vy)

def find_vx_vy(hull, origin, v1, v2):    
    """
    Given the convex hull of points, the origin and two vectors v1 and v2, 
    determine whether v1 == vx && v2 == vy or vice versa.
    """
    # find the index of the origin in the points list
    origin_index = 0
    for v in hull.vertices:
        if np.array_equal(hull.points[v], origin):
            origin_index = v
    
    # roll the vertices to start with the origin. vx will always be to the right of the origin, vy will be left
    verts = np.roll(hull.vertices, -np.asscalar(np.where(hull.vertices == origin_index)[0]))

    for v in verts:
        if np.array_equal(hull.points[v], v1):
            return v1, v2
            
        if np.array_equal(hull.points[v], v2):
            return v2, v1
            vx = v2


def norm(reference, point):
    """Given a reference system (o, vx, vy), normalize a set of points to new coordinates."""
    o = reference[0]
    x = reference[1]
    y = reference[2]
    s = point
    
    # Richtungsvektoren entlang der Kanten vom Referenzsystem
    vx = x - o
    vy = y - o
    # Ortsvektor des Punkts bzgl o (wo ist s bgzl des neuen Ursprungs o)
    so = s - o
    
    # Normierung der Richtungsvektoren    
    vx = (vx/(np.linalg.norm(vx)))/(np.linalg.norm(vx))*3
    vy = (vy/(np.linalg.norm(vy)))/(np.linalg.norm(vy))*3
   
    xcor = np.dot(vx, so)
    ycor = np.dot(vy, so)
    
    log("s.x: {0}, s.y: {1}".format(xcor, ycor))
    
    return (round(xcor, 1), round(ycor, 1))


def touchcode_from_points(points):   
    """Generate touchcode for a set of normalized touchpoints."""
    
    touchcode = 0
    touchpoint_map = {
        (1,3): 0x001,
        (2,3): 0x002,
        (0,2): 0x004,
        (1,2): 0x008,
        (2,2): 0x010,
        (3,2): 0x020,
        (0,1): 0x040,
        (1,1): 0x080,
        (2,1): 0x100,
        (3,1): 0x200,
        (1,0): 0x400,
        (2,0): 0x800
    }
    
    for touchpoint, tc_bit in touchpoint_map.items():
        if any(map(lambda point: approximates(touchpoint, point, 0.2), points)):
            touchcode |= tc_bit
    
    return touchcode


def check_touchcode(points):
    """Main API function. Takes a list of points, finds the reference system in it and tries to decode 
    the corresponding touchcode.
    
    Returns: A touchcode from 0 to 4095 (12 bit) or -1 if no touchcode could be decoded.
    """
    no_result = -1
    
    if points is None or not isinstance(points, list):
        return no_result
    
    reference_system = get_orientation_marks(points)
     
    if reference_system is None:
        return no_result
    
    touchpoints = [norm(reference_system, point) for point in points]
    
    return touchcode_from_points(touchpoints)

def check_touchcode_str(coord_string):
    """
    Wrapper around check_touchcode_lst to make it externally callable with a string of coordinates.
    """
    
    return check_touchcode(string_to_coords(coord_string))