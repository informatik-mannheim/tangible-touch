import sys

if __name__ == "__main__":
	try:
		print(sys.argv[1])
		sys.exit(0x80)
	except Exception as e:	
		sys.exit(-1)
