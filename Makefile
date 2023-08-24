all:
	@echo "use option\n\
	[docs]: generate code documentation in doxygen (please install doxygen in your pc)"
docs:
	doxygen ./doc/Doxyfile