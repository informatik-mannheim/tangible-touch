import collections
import itertools
import numpy as np
from operator import itemgetter
from scipy.spatial import distance
import re

DEBUG = False

"""Vector stuff"""

def v_same_orientation(v1, v2):
    return np.dot(v1, v2) > 0
    
"""Division by zero problem!"""
def v_angle(v1, v2):
    length_v1 = np.linalg.norm(v1)
    length_v2 = np.linalg.norm(v2)
    
    if length_v1 == 0 or length_v2 == 0:
        return 0
    
    return np.round(np.degrees(np.arccos(np.dot(v1, v2) /  length_v1 * length_v2)))

def v_perpendicular(v1, v2, tolerance_deg = 0):
    return in_range(v_angle(v1, v2), 90, 5)

def v_parallel(v1, v2, tolerance_deg = 0):       
    return in_range(v_angle(v1, v2), 0, 5) or in_range(v_angle(v1, v2), 180, 5)

def in_range(value, target, tolerance):
    return target - tolerance <= value <= target + tolerance

def v_rotate(matrix, angle):
    """rotates the given matrix by angle in degrees, counter clockwise."""
    angle = np.radians(angle)
    rot_matrix = np.array( [ [ np.cos(angle), -np.sin(angle)], [ np.sin(angle), np.cos(angle)] ] )
    return np.dot(rot_matrix, matrix)
    

"""Helpers"""

def log(message):
    """Prints a message only if DEBUG = True, so that all printing to stdout can be easily disabled."""
    if DEBUG:
        print(message)

def are_same(reference, value, percentage):
    min_value = reference - reference * percentage
    max_value = reference + reference * percentage
    result = min_value < value < max_value
    
    return result

"""Heavy stuff"""

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
    
    Returns None if no reference system found.
    """
    p_threshold = 0.10

    # no touchcode if there are not enough points
    if points is None or len(points) < 3:
        return None
    
    # calculate all possible distances between all points
    vectors = [(p1, p2, distance.euclidean(p1, p2)) for p1, p2 in list(itertools.combinations(points, 2))]

    # get the two points that have the longest distance (those are vx and vy)
    v1, v2, longest_distance = max(vectors, key=itemgetter(2))
    
    log("v1: {0}, v2: {1}, dst(v1, v2): {2}]".format(v1, v2, longest_distance))
    
    origin = None
    candidates = []
    
    # find the origin candidates by getting all distances that are longest_distance / sqrt(2)    
    for vector in vectors:
        if are_same(longest_distance / np.sqrt(2), vector[2], p_threshold):
            if np.array_equal(vector[0], v1) or np.array_equal(vector[0], v2):
                candidates.append((vector[1][0], vector[1][1]))
            if np.array_equal(vector[1], v1) or np.array_equal(vector[1], v2):
                candidates.append((vector[0][0], vector[0][1]))
    
    # find the origin (the point that we got twice)
    try:
        origin = np.array([k for k, v in collections.Counter(candidates).items() if v == 2])[0]
    except:
        return None
    
    return find_vx_vy_new(np.array([origin,v1,v2]))

def find_vx_vy_new(m):
    """
    Given three points (origin, v1, v2), finds out which of v1, v2 is vx and vy.
    
    Input: A 2x3 matrix (origin, v1, v2)
    Output: A 2x3 matrix (origin, vx, vy)
    """
    
    # The standard coordinate system
    positive_x = np.array([1,0])
    positive_y = np.array([0,1])
    real_origin = np.array([0,0])
    
    # The origin of our touchcode system
    origin = m[0]
    
    # Translate the touchcode coordinate system to have its origin at the standard origin (0,0)
    translation_vec = real_origin - origin
    mt = m + translation_vec
    
    v1, v2 = mt[1], mt[2]
    log("v1 is {0}".format(v1))
    
    # Pick v1 as a pivot and check if it is in first or fourth quadrant.
    # If so, rotate by angle(v1, positive_y) to align v2 with the x-axis.
    # Next, check whether v2 has the same orientation as the positive x-axis, v1 then being vx. 
    # In the other case, v1 is the vx.
    if v_same_orientation(v1, positive_x):
        log("v1 is oriented with positive_x")
        angle = v_angle(v1, positive_y)
        log("angle: {0}".format(angle))
        v1 = v_rotate(v1, angle)
        v2 = v_rotate(v2, angle)
    else:
        log("v1 is NOT oriented with positive_x")
        angle = 360 - v_angle(v1, positive_y)
        v1 = v_rotate(v1, angle)
        v2 = v_rotate(v2, angle)
    
    log(v_same_orientation(v2, positive_x))
    log("after rot: v1 = {0} and v2 = {1}".format(v1, v2))
    if v_same_orientation(v2, positive_x):
        return np.array([m[0],m[2],m[1]])
    else:
        return m

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

def xmirror(points, max_y):
    mirrored_points = []
       
    for point in points:
        mirrored_points.append((point[0], max_y - point[1]))
        
    return mirrored_points

def check_touchcode(points, x_mirror=True, max_y=1080):
    """Main API function. Takes a list of points, finds the reference system in it and tries to decode 
    the corresponding touchcode.
    
    Returns: A touchcode from 0 to 4095 (12 bit) or -1 if no touchcode could be decoded.
    """       
    no_result = -1
    
    if points is None or not isinstance(points, list):
        return no_result
    
    if x_mirror:
        points = xmirror(points, max_y)
    
    reference_system = get_orientation_marks(points)
     
    if reference_system is None:
        return no_result
    
    touchpoints = [norm(reference_system, point) for point in points]
    
    return touchcode_from_points(touchpoints)

def check_touchcode_str(coord_string, x_mirror=True):
    """
    Wrapper around check_touchcode_lst to make it externally callable with a string of coordinates.
    """
    
    return check_touchcode(string_to_coords(coord_string), x_mirror)