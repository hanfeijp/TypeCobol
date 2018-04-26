﻿       IDENTIFICATION DIVISION.
       PROGRAM-ID. PGM1.
       PROCEDURE DIVISION.      
      *-----------------------------------------------------------------
       declare procedure StartCheckpoint public
               input param1 pic X.
       end-declare.
       END PROGRAM PGM1.

       IDENTIFICATION DIVISION.
       PROGRAM-ID. PGM2.
       PROCEDURE DIVISION.      
      *-----------------------------------------------------------------
       declare procedure CheckContract public
               input param1 pic X.
       procedure division.
           call PGM1::StartCheckpoint input param1
           .
       end-declare.
       END PROGRAM PGM2.

       IDENTIFICATION DIVISION.
       PROGRAM-ID. MyPGM.
       PROCEDURE DIVISION.      
       declare procedure testos private.
       data division.
       working-storage section.
       01 param1 pic X.
       procedure division.
                               call PGM1::StartCheckpoint input param1
           .
       end-declare.
       END PROGRAM MyPGM.
