﻿using System.Linq;

namespace TypeCobol.Compiler.Scanner
{
    // WARNING : both enumerations below (families / types) must stay in sync
    // WARNING : make sure to update the tables in TokenUtils if you add one more token family or one more token type

    public enum TokenFamily
    {
        // 0 -> 0 : Error
        Invalid = 0,

        // 1 -> 3 : Whitespace
        // p46: The separator comma and separator semicolon can
        // be used anywhere the separator space is used.
        Whitespace = 1,

        // 4 -> 5 : Comments
        Comments = 4,

        // 6 -> 11 : Separators - Syntax
        SyntaxSeparator = 6,

        // 12 -> 16 : Special character word - Arithmetic operators
        ArithmeticOperator = 12,

        // 17 -> 21 : Special character word - Relational operators
        RelationalOperator = 17,

        // 22 -> 27 : Literals - Alphanumeric
        AlphanumericLiteral = 22,

        // 28 -> 31 : Literals - Numeric
        NumericLiteral = 28,

        // 32 -> 34 : Literals - Syntax tokens
        SyntaxLiteral = 32,

        // 35 -> 39 : Symbols
        Symbol = 35,

        // 40 -> 58 : Keywords - Compiler directive starting tokens
        CompilerDirectiveStartingKeyword = 40,

        // 59 -> 154 : Keywords - Code element starting tokens
        CodeElementStartingKeyword = 59,

        // 155 -> 187 : Keywords - Special registers
        SpecialRegisterKeyword = 155,

        // 188 -> 201 : Keywords - Figurative constants
        FigurativeConstantKeyword = 188,

        // 202 -> 203 : Keywords - Special object identifiers
        SpecialObjectIdentifierKeyword = 202,

        // 204 -> 497 : Keywords - Syntax tokens
        SyntaxKeyword = 204,

        // 498 -> 500 : Keywords - Cobol V6
        CobolV6Keyword = 498,

        // 501 -> 502 : Keywords - Cobol 2002
        Cobol2002Keyword = 501,

        // 503 -> 507 : Keywords - TypeCobol
        TypeCobolKeyword = 503,

        // 508 -> 508 : TypeCobol Operators
        TypeCobolOperators = 508,

        // 509 -> 511 : Compiler directives
        CompilerDirective = 509,

        // 512 -> 512 : Internal token groups (used by the preprocessor only)
        InternalTokenGroup = 512,

        // 513 -> 524 : Formalized Comments Tokens
        FormalizedCommentsFamily = 513,

        // 525 -> 526 : Multilines Comments Tokens
        MultilinesCommentsFamily = 525
    }

    // INFO : the list below is generated from the file Documentation/Studies/CobolLexer.tokens.xls
    //Or See TokenType.xlsx file that contains data to regenerate the TokenType enum 
    // WARNING : the list of tokens in CobolWords.g4 must stay in sync

    public enum TokenType
    {
        EndOfFile = -1,
        InvalidToken = 0,
        SpaceSeparator = 1,
        CommaSeparator = 2,
        SemicolonSeparator = 3,
        FloatingComment = 4,
        CommentLine = 5,
        PeriodSeparator = 6,
        ColonSeparator = 7,
        QualifiedNameSeparator = 8,
        LeftParenthesisSeparator = 9,
        RightParenthesisSeparator = 10,
        PseudoTextDelimiter = 11,
        PlusOperator = 12,
        MinusOperator = 13,
        DivideOperator = 14,
        MultiplyOperator = 15,
        PowerOperator = 16,
        LessThanOperator = 17,
        GreaterThanOperator = 18,
        LessThanOrEqualOperator = 19,
        GreaterThanOrEqualOperator = 20,
        EqualOperator = 21,
        AlphanumericLiteral = 22,
        HexadecimalAlphanumericLiteral = 23,
        NullTerminatedAlphanumericLiteral = 24,
        NationalLiteral = 25,
        HexadecimalNationalLiteral = 26,
        DBCSLiteral = 27,
        LevelNumber = 28,
        IntegerLiteral = 29,
        DecimalLiteral = 30,
        FloatingPointLiteral = 31,
        PictureCharacterString = 32,
        CommentEntry = 33,
        ExecStatementText = 34,
        SectionParagraphName = 35,
        IntrinsicFunctionName = 36,
        ExecTranslatorName = 37,
        PartialCobolWord = 38,
        UserDefinedWord = 39,
        ASTERISK_CBL = 40,
        ASTERISK_CONTROL = 41,
        BASIS = 42,
        CBL = 43,
        COPY = 44,
        DELETE_CD = 45,
        EJECT = 46,
        ENTER = 47,
        EXEC_SQL = 48,
        INSERT = 49,
        PROCESS = 50,
        READY = 51,
        RESET = 52,
        REPLACE = 53,
        SERVICE_CD = 54,
        SKIP1 = 55,
        SKIP2 = 56,
        SKIP3 = 57,
        TITLE = 58,
        ACCEPT = 59,
        ADD = 60,
        ALTER = 61,
        APPLY = 62,
        CALL = 63,
        CANCEL = 64,
        CLOSE = 65,
        COMPUTE = 66,
        CONFIGURATION = 67,
        CONTINUE = 68,
        DATA = 69,
        DECLARATIVES = 70,
        DECLARE = 71,
        DELETE = 72,
        DISPLAY = 73,
        DIVIDE = 74,
        ELSE = 75,
        END = 76,
        END_ADD = 77,
        END_CALL = 78,
        END_COMPUTE = 79,
        END_DECLARE = 80,
        END_DELETE = 81,
        END_DIVIDE = 82,
        END_EVALUATE = 83,
        END_EXEC = 84,
        END_IF = 85,
        END_INVOKE = 86,
        END_MULTIPLY = 87,
        END_PERFORM = 88,
        END_READ = 89,
        END_RETURN = 90,
        END_REWRITE = 91,
        END_SEARCH = 92,
        END_START = 93,
        END_STRING = 94,
        END_SUBTRACT = 95,
        END_UNSTRING = 96,
        END_WRITE = 97,
        END_XML = 98,
        ENTRY = 99,
        ENVIRONMENT = 100,
        EVALUATE = 101,
        EXEC = 102,
        EXECUTE = 103,
        EXIT = 104,
        FD = 105,
        FILE = 106,
        FILE_CONTROL = 107,
        GO = 108,
        GOBACK = 109,
        I_O_CONTROL = 110,
        ID = 111,
        IDENTIFICATION = 112,
        IF = 113,
        INITIALIZE = 114,
        INPUT_OUTPUT = 115,
        INSPECT = 116,
        INVOKE = 117,
        LINKAGE = 118,
        LOCAL_STORAGE = 119,
        MERGE = 120,
        MOVE = 121,
        MULTIPLE = 122,
        MULTIPLY = 123,
        NEXT = 124,
        OBJECT_COMPUTER = 125,
        OPEN = 126,
        PERFORM = 127,
        PROCEDURE = 128,
        READ = 129,
        RELEASE = 130,
        REPOSITORY = 131,
        RERUN = 132,
        RETURN = 133,
        REWRITE = 134,
        SAME = 135,
        SD = 136,
        SEARCH = 137,
        SELECT = 138,
        SERVICE = 139,
        SET = 140,
        SORT = 141,
        SOURCE_COMPUTER = 142,
        SPECIAL_NAMES = 143,
        START = 144,
        STOP = 145,
        STRING = 146,
        SUBTRACT = 147,
        UNSTRING = 148,
        USE = 149,
        WHEN = 150,
        WORKING_STORAGE = 151,
        WRITE = 152,
        XML = 153,
        GLOBAL_STORAGE = 154,
        ADDRESS = 155,
        DEBUG_CONTENTS = 156,
        DEBUG_ITEM = 157,
        DEBUG_LINE = 158,
        DEBUG_NAME = 159,
        DEBUG_SUB_1 = 160,
        DEBUG_SUB_2 = 161,
        DEBUG_SUB_3 = 162,
        JNIENVPTR = 163,
        JSON_CODE = 164,
        JSON_STATUS = 165,
        LENGTH = 166,
        LINAGE_COUNTER = 167,
        RETURN_CODE = 168,
        SHIFT_IN = 169,
        SHIFT_OUT = 170,
        SORT_CONTROL = 171,
        SORT_CORE_SIZE = 172,
        SORT_FILE_SIZE = 173,
        SORT_MESSAGE = 174,
        SORT_MODE_SIZE = 175,
        SORT_RETURN = 176,
        TALLY = 177,
        WHEN_COMPILED = 178,
        XML_CODE = 179,
        XML_EVENT = 180,
        XML_INFORMATION = 181,
        XML_NAMESPACE = 182,
        XML_NAMESPACE_PREFIX = 183,
        XML_NNAMESPACE = 184,
        XML_NNAMESPACE_PREFIX = 185,
        XML_NTEXT = 186,
        XML_TEXT = 187,
        HIGH_VALUE = 188,
        HIGH_VALUES = 189,
        LOW_VALUE = 190,
        LOW_VALUES = 191,
        NULL = 192,
        NULLS = 193,
        QUOTE = 194,
        QUOTES = 195,
        SPACE = 196,
        SPACES = 197,
        ZERO = 198,
        ZEROES = 199,
        ZEROS = 200,
        SymbolicCharacter = 201,
        SELF = 202,
        SUPER = 203,
        ACCESS = 204,
        ADVANCING = 205,
        AFTER = 206,
        ALL = 207,
        ALPHABET = 208,
        ALPHABETIC = 209,
        ALPHABETIC_LOWER = 210,
        ALPHABETIC_UPPER = 211,
        ALPHANUMERIC = 212,
        ALPHANUMERIC_EDITED = 213,
        ALSO = 214,
        ALTERNATE = 215,
        AND = 216,
        ANY = 217,
        ARE = 218,
        AREA = 219,
        AREAS = 220,
        ASCENDING = 221,
        ASSIGN = 222,
        AT = 223,
        AUTHOR = 224,
        BEFORE = 225,
        BEGINNING = 226,
        BINARY = 227,
        BLANK = 228,
        BLOCK = 229,
        BOTTOM = 230,
        BY = 231,
        CHARACTER = 232,
        CHARACTERS = 233,
        CLASS = 234,
        CLASS_ID = 235,
        COBOL = 236,
        CODE = 237,
        CODE_SET = 238,
        COLLATING = 239,
        COM_REG = 240,
        COMMA = 241,
        COMMON = 242,
        COMP = 243,
        COMP_1 = 244,
        COMP_2 = 245,
        COMP_3 = 246,
        COMP_4 = 247,
        COMP_5 = 248,
        COMPUTATIONAL = 249,
        COMPUTATIONAL_1 = 250,
        COMPUTATIONAL_2 = 251,
        COMPUTATIONAL_3 = 252,
        COMPUTATIONAL_4 = 253,
        COMPUTATIONAL_5 = 254,
        CONTAINS = 255,
        CONTENT = 256,
        CONVERTING = 257,
        CORR = 258,
        CORRESPONDING = 259,
        COUNT = 260,
        CURRENCY = 261,
        DATE = 262,
        DATE_COMPILED = 263,
        DATE_WRITTEN = 264,
        DAY = 265,
        DAY_OF_WEEK = 266,
        DBCS = 267,
        DEBUGGING = 268,
        DECIMAL_POINT = 269,
        DELIMITED = 270,
        DELIMITER = 271,
        DEPENDING = 272,
        DESCENDING = 273,
        DISPLAY_1 = 274,
        DIVISION = 275,
        DOWN = 276,
        DUPLICATES = 277,
        DYNAMIC = 278,
        EGCS = 279,
        END_OF_PAGE = 280,
        ENDING = 281,
        EOP = 282,
        EQUAL = 283,
        ERROR = 284,
        EVERY = 285,
        EXCEPTION = 286,
        EXTEND = 287,
        EXTERNAL = 288,
        FACTORY = 289,
        FALSE = 290,
        FILLER = 291,
        FIRST = 292,
        FOOTING = 293,
        FOR = 294,
        FROM = 295,
        FUNCTION = 296,
        FUNCTION_POINTER = 297,
        GENERATE = 298,
        GIVING = 299,
        GLOBAL = 300,
        GREATER = 301,
        GROUP_USAGE = 302,
        I_O = 303,
        IN = 304,
        INDEX = 305,
        INDEXED = 306,
        INHERITS = 307,
        INITIAL = 308,
        INPUT = 309,
        INSTALLATION = 310,
        INTO = 311,
        INVALID = 312,
        IS = 313,
        JUST = 314,
        JUSTIFIED = 315,
        KANJI = 316,
        KEY = 317,
        LABEL = 318,
        LEADING = 319,
        LEFT = 320,
        LESS = 321,
        LINAGE = 322,
        LINE = 323,
        LINES = 324,
        LOCK = 325,
        MEMORY = 326,
        METHOD = 327,
        METHOD_ID = 328,
        MODE = 329,
        MODULES = 330,
        MORE_LABELS = 331,
        NATIONAL = 332,
        NATIONAL_EDITED = 333,
        NATIVE = 334,
        NEGATIVE = 335,
        NEW = 336,
        NO = 337,
        NOT = 338,
        NUMERIC = 339,
        NUMERIC_EDITED = 340,
        OBJECT = 341,
        OCCURS = 342,
        OF = 343,
        OFF = 344,
        OMITTED = 345,
        ON = 346,
        OPTIONAL = 347,
        OR = 348,
        ORDER = 349,
        ORGANIZATION = 350,
        OTHER = 351,
        OUTPUT = 352,
        OVERFLOW = 353,
        OVERRIDE = 354,
        PACKED_DECIMAL = 355,
        PADDING = 356,
        PAGE = 357,
        PASSWORD = 358,
        PIC = 359,
        PICTURE = 360,
        POINTER = 361,
        POSITION = 362,
        POSITIVE = 363,
        PROCEDURE_POINTER = 364,
        PROCEDURES = 365,
        PROCEED = 366,
        PROCESSING = 367,
        PROGRAM = 368,
        PROGRAM_ID = 369,
        RANDOM = 370,
        RECORD = 371,
        RECORDING = 372,
        RECORDS = 373,
        RECURSIVE = 374,
        REDEFINES = 375,
        REEL = 376,
        REFERENCE = 377,
        REFERENCES = 378,
        RELATIVE = 379,
        RELOAD = 380,
        REMAINDER = 381,
        REMOVAL = 382,
        RENAMES = 383,
        REPLACING = 384,
        RESERVE = 385,
        RETURNING = 386,
        REVERSED = 387,
        REWIND = 388,
        RIGHT = 389,
        ROUNDED = 390,
        RUN = 391,
        SECTION = 392,
        SECURITY = 393,
        SEGMENT_LIMIT = 394,
        SENTENCE = 395,
        SEPARATE = 396,
        SEQUENCE = 397,
        SEQUENTIAL = 398,
        SIGN = 399,
        SIZE = 400,
        SORT_MERGE = 401,
        SQL = 402,
        SQLIMS = 403,
        STANDARD = 404,
        STANDARD_1 = 405,
        STANDARD_2 = 406,
        STATUS = 407,
        SUPPRESS = 408,
        SYMBOL = 409,
        SYMBOLIC = 410,
        SYNC = 411,
        SYNCHRONIZED = 412,
        TALLYING = 413,
        TAPE = 414,
        TEST = 415,
        THAN = 416,
        THEN = 417,
        THROUGH = 418,
        THRU = 419,
        TIME = 420,
        TIMES = 421,
        TO = 422,
        TOP = 423,
        TRACE = 424,
        TRAILING = 425,
        TRUE = 426,
        TYPE = 427,
        UNBOUNDED = 428,
        UNIT = 429,
        UNTIL = 430,
        UP = 431,
        UPON = 432,
        USAGE = 433,
        USING = 434,
        VALUE = 435,
        VALUES = 436,
        VARYING = 437,
        WITH = 438,
        WORDS = 439,
        WRITE_ONLY = 440,
        XML_SCHEMA = 441,
        ALLOCATE = 442,
        CD = 443,
        CF = 444,
        CH = 445,
        CLOCK_UNITS = 446,
        COLUMN = 447,
        COMMUNICATION = 448,
        CONTROL = 449,
        CONTROLS = 450,
        DE = 451,
        DEFAULT = 452,
        DESTINATION = 453,
        DETAIL = 454,
        DISABLE = 455,
        EGI = 456,
        EMI = 457,
        ENABLE = 458,
        END_RECEIVE = 459,
        ESI = 460,
        FINAL = 461,
        FREE = 462,
        GROUP = 463,
        HEADING = 464,
        INDICATE = 465,
        INITIATE = 466,
        LAST = 467,
        LIMIT = 468,
        LIMITS = 469,
        LINE_COUNTER = 470,
        MESSAGE = 471,
        NUMBER = 472,
        PAGE_COUNTER = 473,
        PF = 474,
        PH = 475,
        PLUS = 476,
        PRINTING = 477,
        PURGE = 478,
        QUEUE = 479,
        RD = 480,
        RECEIVE = 481,
        REPORT = 482,
        REPORTING = 483,
        REPORTS = 484,
        RF = 485,
        RH = 486,
        SEGMENT = 487,
        SEND = 488,
        SOURCE = 489,
        SUB_QUEUE_1 = 490,
        SUB_QUEUE_2 = 491,
        SUB_QUEUE_3 = 492,
        SUM = 493,
        TABLE = 494,
        TERMINAL = 495,
        TERMINATE = 496,
        TEXT = 497,
        END_JSON = 498,
        JSON = 499,
        VOLATILE = 500,
        TYPEDEF = 501,
        STRONG = 502,
        UNSAFE = 503,
        PUBLIC = 504,
        PRIVATE = 505,
        IN_OUT = 506,
        STRICT = 507,
        QUESTION_MARK = 508,
        COMPILER_DIRECTIVE = 509,
        COPY_IMPORT_DIRECTIVE = 510,
        REPLACE_DIRECTIVE = 511,
        CONTINUATION_TOKEN_GROUP = 512,
        FORMALIZED_COMMENTS_START = 513,
        FORMALIZED_COMMENTS_STOP = 514,
        FORMALIZED_COMMENTS_DESCRIPTION = 515,
        FORMALIZED_COMMENTS_PARAMETERS = 516,
        FORMALIZED_COMMENTS_DEPRECATED = 517,
        FORMALIZED_COMMENTS_REPLACED_BY = 518,
        FORMALIZED_COMMENTS_RESTRICTION = 519,
        FORMALIZED_COMMENTS_NEED = 520,
        FORMALIZED_COMMENTS_SEE = 521,
        FORMALIZED_COMMENTS_TODO = 522,
        FORMALIZED_COMMENTS_VALUE = 523,
        AT_SIGN = 524,
        MULTILINES_COMMENTS_START = 525,
        MULTILINES_COMMENTS_STOP = 526,
    }
}
