grammar statement;

statement:
    sequence;



group:
    token |
    repeat
    ;

sequence:
    or_sequence | and_sequence | optional_sequence | optional_repeated_sequence | required_sequence;

or_sequence:
    group (O_PIPE group)*;
and_sequence:
    group+;

optional_sequence:
    O_OPENO sequence O_CLOSEO;

required_sequence:
    O_OPENR sequence O_CLOSER;

optional_repeated_sequence:
    O_OPENO O_COMMA sequence O_CLOSEO;

token: M_TOKEN;
repeat: O_3DOT;

M_TOKEN: [a-zA-Z_\@\.'][a-zA-Z_\@\.0-9']*;
O_PIPE: '|';
O_AT: '@';
O_COLON: ':';
O_COMMA: ',';
O_DOT: '.';
O_3DOT: '...';
O_OPENO: '[';
O_CLOSEO: ']'; 
O_DASH: '-';
O_OPENR: '{';
O_CLOSER: '}'; 


fragment DIGIT : [0-9];
fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];

WHITESPACE : ( '\t' | ' ' | '\r' | '\n'| '\u000C' )+ -> skip ;