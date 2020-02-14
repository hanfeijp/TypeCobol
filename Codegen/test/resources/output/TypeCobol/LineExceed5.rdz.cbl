       IDENTIFICATION DIVISION.
       PROGRAM-ID. PGM1.
       DATA DIVISION.
                                                         
       WORKING-STORAGE SECTION.

       
       LINKAGE SECTION.
       01 TC-FunctionCode pic X(30).
      * Function which call program f1c0385c
      * Which is generated code for PGM1.StartCheckpoint
           88 Fct-f1c0385c-StartCheckpoint
              value 'Fct=f1c0385c-StartCheckpoint'.

       01 arg1 pic X.
       PROCEDURE DIVISION USING TC-FunctionCode
                           arg1.

           PERFORM INIT-LIBRARY
           PERFORM FctList-Process-Mode
           GOBACK.

       FctList-Process-Mode.
           evaluate true
               when Fct-f1c0385c-StartCheckpoint
                  call 'f1c0385c' using arg1
               when other
                  TODO
           end-evaluate
           .
                                
      *-----------------------------------------------------------------
      *declare procedure StartCheckpoint public
      *        input param1 pic X.
       END PROGRAM PGM1.

       IDENTIFICATION DIVISION.
       PROGRAM-ID. PGM2.
       DATA DIVISION.
                                                         
       WORKING-STORAGE SECTION.

       
       LINKAGE SECTION.
       01 TC-FunctionCode pic X(30).
      * Function which call program f73481e6
      * Which is generated code for PGM2.CheckContract
           88 Fct-f73481e6-CheckContract
              value 'Fct=f73481e6-CheckContract'.

       01 arg1 pic X.
       PROCEDURE DIVISION USING TC-FunctionCode
                           arg1.

           PERFORM INIT-LIBRARY
           PERFORM FctList-Process-Mode
           GOBACK.

       FctList-Process-Mode.
           evaluate true
               when Fct-f73481e6-CheckContract
                  call 'f73481e6' using arg1
               when other
                  TODO
           end-evaluate
           .
                                
      *-----------------------------------------------------------------
      *declare procedure CheckContract public
      *        input param1 pic X.
       END PROGRAM PGM2.

       IDENTIFICATION DIVISION.
       PROGRAM-ID. MyPGM.
       PROCEDURE DIVISION.      
      *declare procedure testos private.
       END PROGRAM MyPGM.
      *
      *declare procedure StartCheckpoint public
      *        input param1 pic X.
      *_________________________________________________________________
       IDENTIFICATION DIVISION.
       PROGRAM-ID. f1c0385c.
       END PROGRAM f1c0385c.
      *
      *declare procedure CheckContract public
      *        input param1 pic X.
      *_________________________________________________________________
       IDENTIFICATION DIVISION.
       PROGRAM-ID. f73481e6.
       DATA DIVISION.
       WORKING-STORAGE SECTION.
      *PGM1.CheckContract - Params :
      *     input(param1: pic X)
       01 TC-PGM1 pic X(08) value 'PGM1'.

       01 TC-Call          PIC X     VALUE 'T'.
           88 TC-FirstCall  VALUE 'T'.
           88 TC-NthCall    VALUE 'F'
                            X'00' thru 'S'
                            'U' thru X'FF'.
       LINKAGE SECTION.
      *PGM1.CheckContract - Params :
      *     input(param1: pic X)
      *Common to all librairies used by the program.
       01 TC-Library-PntTab.
           05 TC-Library-PntNbr          PIC S9(04) COMP.
           05 TC-Library-Item OCCURS 1000
                               DEPENDING ON TC-Library-PntNbr
                               INDEXED   BY TC-Library-Idx.
              10 TC-Library-Item-Idt      PIC X(08).
              10 TC-Library-Item-Pnt      PROCEDURE-POINTER.

      *To call program f1c0385c in module PGM1
      *Which is generated code for PGM1.StartCheckpoint
      *Declared in source file LineExceed5.rdz.cbl
       01 TC-PGM1-f1c0385c-Item.
          05 TC-PGM1-f1c0385c-Idt PIC X(08).
          05 TC-PGM1-f1c0385c PROCEDURE-POINTER.
       01 param1 pic X.
       PROCEDURE DIVISION
             USING BY REFERENCE param1
           .
      *PGM1.CheckContract - Params :
      *     input(param1: pic X)
           PERFORM TC-INITIALIZATIONS
      *    call PGM1::StartCheckpoint input param1
           CALL 'zcallpgm' using TC-PGM1
                    PGM1-Fct-f1c0385c-StartCheckpo
                                 param1
           end-call
                                                  
           .
      *=================================================================
       TC-INITIALIZATIONS.
      *=================================================================
            IF TC-FirstCall
                 SET TC-NthCall TO TRUE
                 SET ADDRESS OF TC-PGM1-f1c0385c-Item  TO NULL
            END-IF
            .
      *=================================================================
       TC-LOAD-POINTERS-PGM1.
      *=================================================================
            CALL 'ZCALLPGM' USING TC-PGM1
            ADDRESS OF TC-Library-PntTab
            PERFORM VARYING TC-Library-Idx FROM 1 BY 1
            UNTIL TC-Library-Idx > TC-Library-PntNbr
                EVALUATE TC-Library-Item-Idt (TC-Library-Idx)
                WHEN 'f1c0385c'
                     SET ADDRESS OF
                     TC-PGM1-f1c0385c-Item
                     TO ADDRESS OF
                     TC-Library-Item(TC-Library-Idx)
                WHEN OTHER
                     CONTINUE
                END-EVALUATE
            END-PERFORM
            .
       END PROGRAM f73481e6.
      *
      *declare procedure testos private.
      *_________________________________________________________________
       IDENTIFICATION DIVISION.
       PROGRAM-ID. bfc74757.
       data division.
       working-storage section.
      *PGM1.testos  - No Params
                               
       01 TC-PGM1 pic X(08) value 'PGM1'.

       01 TC-Call          PIC X     VALUE 'T'.
           88 TC-FirstCall  VALUE 'T'.
           88 TC-NthCall    VALUE 'F'
                            X'00' thru 'S'
                            'U' thru X'FF'.
       01 param1 pic X.
       LINKAGE SECTION.
      *PGM1.testos  - No Params
      *Common to all librairies used by the program.
       01 TC-Library-PntTab.
           05 TC-Library-PntNbr          PIC S9(04) COMP.
           05 TC-Library-Item OCCURS 1000
                               DEPENDING ON TC-Library-PntNbr
                               INDEXED   BY TC-Library-Idx.
              10 TC-Library-Item-Idt      PIC X(08).
              10 TC-Library-Item-Pnt      PROCEDURE-POINTER.

      *To call program f1c0385c in module PGM1
      *Which is generated code for PGM1.StartCheckpoint
      *Declared in source file LineExceed5.rdz.cbl
       01 TC-PGM1-f1c0385c-Item.
          05 TC-PGM1-f1c0385c-Idt PIC X(08).
          05 TC-PGM1-f1c0385c PROCEDURE-POINTER.
       PROCEDURE DIVISION
           .
      *PGM1.testos  - No Params
           PERFORM TC-INITIALIZATIONS
      *                        call PGM1::StartCheckpoint input param1
                               CALL 'zcallpgm' using TC-PGM1
                    PGM1-Fct-f1c0385c-StartCheckpo
                                 param1
           end-call
                                                                      
           .
      *=================================================================
       TC-INITIALIZATIONS.
      *=================================================================
            IF TC-FirstCall
                 SET TC-NthCall TO TRUE
                 SET ADDRESS OF TC-PGM1-f1c0385c-Item  TO NULL
            END-IF
            .
      *=================================================================
       TC-LOAD-POINTERS-PGM1.
      *=================================================================
            CALL 'ZCALLPGM' USING TC-PGM1
            ADDRESS OF TC-Library-PntTab
            PERFORM VARYING TC-Library-Idx FROM 1 BY 1
            UNTIL TC-Library-Idx > TC-Library-PntNbr
                EVALUATE TC-Library-Item-Idt (TC-Library-Idx)
                WHEN 'f1c0385c'
                     SET ADDRESS OF
                     TC-PGM1-f1c0385c-Item
                     TO ADDRESS OF
                     TC-Library-Item(TC-Library-Idx)
                WHEN OTHER
                     CONTINUE
                END-EVALUATE
            END-PERFORM
            .
       END PROGRAM bfc74757.
