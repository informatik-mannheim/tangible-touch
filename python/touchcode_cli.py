import sys
from touchcode import check_touchcode_str
  
if __name__ == "__main__":
	if len(sys.argv) < 2:
		sys.exit(-1)
		
	touchcode = check_touchcode_str(sys.argv[1])
	print(touchcode)
	
	sys.exit(touchcode)