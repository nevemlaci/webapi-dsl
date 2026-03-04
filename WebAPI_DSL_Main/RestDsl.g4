grammar RestDsl;

program: configBlock (entity)*;

// -- Config --

configBlock
    : 'config' '{' 
        configs += configProperty*
      '}'
    ;

configProperty:
    key=ID ':' value=STRING;

// -- Types -- 

// int, foobar, List<int>
typeName :
    primitiveType | typeid = ID | listType;

//typename including void
typeNameOrVoid:
    typeName | 'void';

//primitive types  
primitiveType: 
    'string' | 'int' | 'double' | 'long' | 'bool';

// List<string>
listType:
    'List' '<' typeName '>';

// enum FooBar { Foo, Bar }
enumDeclaration:
    'enum' name=ID '{' values+=ID ( ',' values+=ID )* ','? '}';

// -- Entity --

entity: 
    annotations+=annotation* 'entity' ID '{' field* '}';

field: 
    annotations+=annotation* name=ID ':' typeoffield=typeName ('=' default=expression)?;

// -- Annotation --
    
annotation:
   name = ANNOTATION parameterAssignmentTuple?
   ;
    
// -- Tuples --

parameterAssignmentTuple:
    '(' params += annotationParameterAssignment (',' params += annotationParameterAssignment ','?)* ')'
    ;

annotationParameterAssignment:
    name = ID '=' atom
    ;

// -- Expressions, atoms --

expression:
    atom;
    
atom:
    '(' expression ')' #ParenExpr
    | value=STRING #StringLiteralExpr
    | value=INT #IntegerLiteral
    | value=FLOAT #FloatLiteral
    ;
    

// -- Terminals --

ANNOTATION : '@' ID ;
HTTPVERB: 'GET' | 'POST' | 'DELETE' | 'PATCH';
INT: [0-9]+;
FLOAT: INT.INT;
ID: [a-zA-Z_][a-zA-Z0-9_]*;
STRING: '"' .*? '"';

WS: [ \t\r\n]+ -> skip; 

LINE_COMMENT
    : '//' ~[\r\n]* -> skip
    ;

BLOCK_COMMENT
    : '/*' .*? '*/' -> skip
    ;