ALLOCATE 9 CHARACTERS RETURNING myPointer
ALLOCATE 9 CHARACTERS INITIALIZED RETURNING myPointer
ALLOCATE 9 CHARACTERS INITIALIZED LOC 24 RETURNING myPointer
ALLOCATE (2 + 7) * 3 CHARACTERS RETURNING myPointer
ALLOCATE (var1 + var2) CHARACTERS INITIALIZED RETURNING myPointer
ALLOCATE (var1 + var2) CHARACTERS INITIALIZED LOC var1 RETURNING myPointer
ALLOCATE var1
ALLOCATE var1 INITIALIZED
ALLOCATE var1 RETURNING myPointer
ALLOCATE var1 INITIALIZED RETURNING myPointer
ALLOCATE var1 LOC 31
* KO required size info missing
ALLOCATE
* KO required size info missing
ALLOCATE INITIALIZED
* KO required CHARACTERS keyword missing
ALLOCATE 12
* KO required CHARACTERS keyword missing
ALLOCATE 12 RETURNING myPointer
* KO required LOC value
ALLOCATE 12 CHARACTERS LOC
* KO extraneous input LOC
ALLOCATE 9 CHARACTERS INITIALIZED RETURNING myPointer LOC 24
* KO no viable alternative at input 'TOTO'
ALLOCATE var1 TOTO 24
