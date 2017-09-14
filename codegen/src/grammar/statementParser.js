// Generated from statement.g4 by ANTLR 4.5.3
// jshint ignore: start
var antlr4 = require('antlr4/index');
var statementListener = require('./statementListener').statementListener;
var statementVisitor = require('./statementVisitor').statementVisitor;

var grammarFileName = "statement.g4";

var serializedATN = ["\u0003\u0430\ud6d1\u8206\uad2d\u4417\uaef1\u8d80\uaadd",
    "\u0003\u000fB\u0004\u0002\t\u0002\u0004\u0003\t\u0003\u0004\u0004\t",
    "\u0004\u0004\u0005\t\u0005\u0004\u0006\t\u0006\u0004\u0007\t\u0007\u0004",
    "\b\t\b\u0004\t\t\t\u0004\n\t\n\u0004\u000b\t\u000b\u0003\u0002\u0003",
    "\u0002\u0003\u0003\u0003\u0003\u0005\u0003\u001b\n\u0003\u0003\u0004",
    "\u0003\u0004\u0003\u0004\u0003\u0004\u0003\u0004\u0005\u0004\"\n\u0004",
    "\u0003\u0005\u0003\u0005\u0003\u0005\u0007\u0005\'\n\u0005\f\u0005\u000e",
    "\u0005*\u000b\u0005\u0003\u0006\u0006\u0006-\n\u0006\r\u0006\u000e\u0006",
    ".\u0003\u0007\u0003\u0007\u0003\u0007\u0003\u0007\u0003\b\u0003\b\u0003",
    "\b\u0003\b\u0003\t\u0003\t\u0003\t\u0003\t\u0003\t\u0003\n\u0003\n\u0003",
    "\u000b\u0003\u000b\u0003\u000b\u0002\u0002\f\u0002\u0004\u0006\b\n\f",
    "\u000e\u0010\u0012\u0014\u0002\u0002>\u0002\u0016\u0003\u0002\u0002",
    "\u0002\u0004\u001a\u0003\u0002\u0002\u0002\u0006!\u0003\u0002\u0002",
    "\u0002\b#\u0003\u0002\u0002\u0002\n,\u0003\u0002\u0002\u0002\f0\u0003",
    "\u0002\u0002\u0002\u000e4\u0003\u0002\u0002\u0002\u00108\u0003\u0002",
    "\u0002\u0002\u0012=\u0003\u0002\u0002\u0002\u0014?\u0003\u0002\u0002",
    "\u0002\u0016\u0017\u0005\u0006\u0004\u0002\u0017\u0003\u0003\u0002\u0002",
    "\u0002\u0018\u001b\u0005\u0012\n\u0002\u0019\u001b\u0005\u0014\u000b",
    "\u0002\u001a\u0018\u0003\u0002\u0002\u0002\u001a\u0019\u0003\u0002\u0002",
    "\u0002\u001b\u0005\u0003\u0002\u0002\u0002\u001c\"\u0005\b\u0005\u0002",
    "\u001d\"\u0005\n\u0006\u0002\u001e\"\u0005\f\u0007\u0002\u001f\"\u0005",
    "\u0010\t\u0002 \"\u0005\u000e\b\u0002!\u001c\u0003\u0002\u0002\u0002",
    "!\u001d\u0003\u0002\u0002\u0002!\u001e\u0003\u0002\u0002\u0002!\u001f",
    "\u0003\u0002\u0002\u0002! \u0003\u0002\u0002\u0002\"\u0007\u0003\u0002",
    "\u0002\u0002#(\u0005\u0004\u0003\u0002$%\u0007\u0004\u0002\u0002%\'",
    "\u0005\u0004\u0003\u0002&$\u0003\u0002\u0002\u0002\'*\u0003\u0002\u0002",
    "\u0002(&\u0003\u0002\u0002\u0002()\u0003\u0002\u0002\u0002)\t\u0003",
    "\u0002\u0002\u0002*(\u0003\u0002\u0002\u0002+-\u0005\u0004\u0003\u0002",
    ",+\u0003\u0002\u0002\u0002-.\u0003\u0002\u0002\u0002.,\u0003\u0002\u0002",
    "\u0002./\u0003\u0002\u0002\u0002/\u000b\u0003\u0002\u0002\u000201\u0007",
    "\n\u0002\u000212\u0005\u0006\u0004\u000223\u0007\u000b\u0002\u00023",
    "\r\u0003\u0002\u0002\u000245\u0007\r\u0002\u000256\u0005\u0006\u0004",
    "\u000267\u0007\u000e\u0002\u00027\u000f\u0003\u0002\u0002\u000289\u0007",
    "\n\u0002\u00029:\u0007\u0007\u0002\u0002:;\u0005\u0006\u0004\u0002;",
    "<\u0007\u000b\u0002\u0002<\u0011\u0003\u0002\u0002\u0002=>\u0007\u0003",
    "\u0002\u0002>\u0013\u0003\u0002\u0002\u0002?@\u0007\t\u0002\u0002@\u0015",
    "\u0003\u0002\u0002\u0002\u0006\u001a!(."].join("");


var atn = new antlr4.atn.ATNDeserializer().deserialize(serializedATN);

var decisionsToDFA = atn.decisionToState.map( function(ds, index) { return new antlr4.dfa.DFA(ds, index); });

var sharedContextCache = new antlr4.PredictionContextCache();

var literalNames = [ null, null, "'|'", "'@'", "':'", "','", "'.'", "'...'", 
                     "'['", "']'", "'-'", "'{'", "'}'" ];

var symbolicNames = [ null, "M_TOKEN", "O_PIPE", "O_AT", "O_COLON", "O_COMMA", 
                      "O_DOT", "O_3DOT", "O_OPENO", "O_CLOSEO", "O_DASH", 
                      "O_OPENR", "O_CLOSER", "WHITESPACE" ];

var ruleNames =  [ "statement", "group", "sequence", "or_sequence", "and_sequence", 
                   "optional_sequence", "required_sequence", "optional_repeated_sequence", 
                   "token", "repeat" ];

function statementParser (input) {
	antlr4.Parser.call(this, input);
    this._interp = new antlr4.atn.ParserATNSimulator(this, atn, decisionsToDFA, sharedContextCache);
    this.ruleNames = ruleNames;
    this.literalNames = literalNames;
    this.symbolicNames = symbolicNames;
    return this;
}

statementParser.prototype = Object.create(antlr4.Parser.prototype);
statementParser.prototype.constructor = statementParser;

Object.defineProperty(statementParser.prototype, "atn", {
	get : function() {
		return atn;
	}
});

statementParser.EOF = antlr4.Token.EOF;
statementParser.M_TOKEN = 1;
statementParser.O_PIPE = 2;
statementParser.O_AT = 3;
statementParser.O_COLON = 4;
statementParser.O_COMMA = 5;
statementParser.O_DOT = 6;
statementParser.O_3DOT = 7;
statementParser.O_OPENO = 8;
statementParser.O_CLOSEO = 9;
statementParser.O_DASH = 10;
statementParser.O_OPENR = 11;
statementParser.O_CLOSER = 12;
statementParser.WHITESPACE = 13;

statementParser.RULE_statement = 0;
statementParser.RULE_group = 1;
statementParser.RULE_sequence = 2;
statementParser.RULE_or_sequence = 3;
statementParser.RULE_and_sequence = 4;
statementParser.RULE_optional_sequence = 5;
statementParser.RULE_required_sequence = 6;
statementParser.RULE_optional_repeated_sequence = 7;
statementParser.RULE_token = 8;
statementParser.RULE_repeat = 9;

function StatementContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_statement;
    return this;
}

StatementContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
StatementContext.prototype.constructor = StatementContext;

StatementContext.prototype.sequence = function() {
    return this.getTypedRuleContext(SequenceContext,0);
};

StatementContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterStatement(this);
	}
};

StatementContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitStatement(this);
	}
};

StatementContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitStatement(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.StatementContext = StatementContext;

statementParser.prototype.statement = function() {

    var localctx = new StatementContext(this, this._ctx, this.state);
    this.enterRule(localctx, 0, statementParser.RULE_statement);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 20;
        this.sequence();
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function GroupContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_group;
    return this;
}

GroupContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
GroupContext.prototype.constructor = GroupContext;

GroupContext.prototype.token = function() {
    return this.getTypedRuleContext(TokenContext,0);
};

GroupContext.prototype.repeat = function() {
    return this.getTypedRuleContext(RepeatContext,0);
};

GroupContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterGroup(this);
	}
};

GroupContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitGroup(this);
	}
};

GroupContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitGroup(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.GroupContext = GroupContext;

statementParser.prototype.group = function() {

    var localctx = new GroupContext(this, this._ctx, this.state);
    this.enterRule(localctx, 2, statementParser.RULE_group);
    try {
        this.state = 24;
        switch(this._input.LA(1)) {
        case statementParser.M_TOKEN:
            this.enterOuterAlt(localctx, 1);
            this.state = 22;
            this.token();
            break;
        case statementParser.O_3DOT:
            this.enterOuterAlt(localctx, 2);
            this.state = 23;
            this.repeat();
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function SequenceContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_sequence;
    return this;
}

SequenceContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
SequenceContext.prototype.constructor = SequenceContext;

SequenceContext.prototype.or_sequence = function() {
    return this.getTypedRuleContext(Or_sequenceContext,0);
};

SequenceContext.prototype.and_sequence = function() {
    return this.getTypedRuleContext(And_sequenceContext,0);
};

SequenceContext.prototype.optional_sequence = function() {
    return this.getTypedRuleContext(Optional_sequenceContext,0);
};

SequenceContext.prototype.optional_repeated_sequence = function() {
    return this.getTypedRuleContext(Optional_repeated_sequenceContext,0);
};

SequenceContext.prototype.required_sequence = function() {
    return this.getTypedRuleContext(Required_sequenceContext,0);
};

SequenceContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterSequence(this);
	}
};

SequenceContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitSequence(this);
	}
};

SequenceContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitSequence(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.SequenceContext = SequenceContext;

statementParser.prototype.sequence = function() {

    var localctx = new SequenceContext(this, this._ctx, this.state);
    this.enterRule(localctx, 4, statementParser.RULE_sequence);
    try {
        this.state = 31;
        this._errHandler.sync(this);
        var la_ = this._interp.adaptivePredict(this._input,1,this._ctx);
        switch(la_) {
        case 1:
            this.enterOuterAlt(localctx, 1);
            this.state = 26;
            this.or_sequence();
            break;

        case 2:
            this.enterOuterAlt(localctx, 2);
            this.state = 27;
            this.and_sequence();
            break;

        case 3:
            this.enterOuterAlt(localctx, 3);
            this.state = 28;
            this.optional_sequence();
            break;

        case 4:
            this.enterOuterAlt(localctx, 4);
            this.state = 29;
            this.optional_repeated_sequence();
            break;

        case 5:
            this.enterOuterAlt(localctx, 5);
            this.state = 30;
            this.required_sequence();
            break;

        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function Or_sequenceContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_or_sequence;
    return this;
}

Or_sequenceContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Or_sequenceContext.prototype.constructor = Or_sequenceContext;

Or_sequenceContext.prototype.group = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(GroupContext);
    } else {
        return this.getTypedRuleContext(GroupContext,i);
    }
};

Or_sequenceContext.prototype.O_PIPE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(statementParser.O_PIPE);
    } else {
        return this.getToken(statementParser.O_PIPE, i);
    }
};


Or_sequenceContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterOr_sequence(this);
	}
};

Or_sequenceContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitOr_sequence(this);
	}
};

Or_sequenceContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitOr_sequence(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.Or_sequenceContext = Or_sequenceContext;

statementParser.prototype.or_sequence = function() {

    var localctx = new Or_sequenceContext(this, this._ctx, this.state);
    this.enterRule(localctx, 6, statementParser.RULE_or_sequence);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 33;
        this.group();
        this.state = 38;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while(_la===statementParser.O_PIPE) {
            this.state = 34;
            this.match(statementParser.O_PIPE);
            this.state = 35;
            this.group();
            this.state = 40;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function And_sequenceContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_and_sequence;
    return this;
}

And_sequenceContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
And_sequenceContext.prototype.constructor = And_sequenceContext;

And_sequenceContext.prototype.group = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(GroupContext);
    } else {
        return this.getTypedRuleContext(GroupContext,i);
    }
};

And_sequenceContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterAnd_sequence(this);
	}
};

And_sequenceContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitAnd_sequence(this);
	}
};

And_sequenceContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitAnd_sequence(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.And_sequenceContext = And_sequenceContext;

statementParser.prototype.and_sequence = function() {

    var localctx = new And_sequenceContext(this, this._ctx, this.state);
    this.enterRule(localctx, 8, statementParser.RULE_and_sequence);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 42; 
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        do {
            this.state = 41;
            this.group();
            this.state = 44; 
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        } while(_la===statementParser.M_TOKEN || _la===statementParser.O_3DOT);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function Optional_sequenceContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_optional_sequence;
    return this;
}

Optional_sequenceContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Optional_sequenceContext.prototype.constructor = Optional_sequenceContext;

Optional_sequenceContext.prototype.O_OPENO = function() {
    return this.getToken(statementParser.O_OPENO, 0);
};

Optional_sequenceContext.prototype.sequence = function() {
    return this.getTypedRuleContext(SequenceContext,0);
};

Optional_sequenceContext.prototype.O_CLOSEO = function() {
    return this.getToken(statementParser.O_CLOSEO, 0);
};

Optional_sequenceContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterOptional_sequence(this);
	}
};

Optional_sequenceContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitOptional_sequence(this);
	}
};

Optional_sequenceContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitOptional_sequence(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.Optional_sequenceContext = Optional_sequenceContext;

statementParser.prototype.optional_sequence = function() {

    var localctx = new Optional_sequenceContext(this, this._ctx, this.state);
    this.enterRule(localctx, 10, statementParser.RULE_optional_sequence);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 46;
        this.match(statementParser.O_OPENO);
        this.state = 47;
        this.sequence();
        this.state = 48;
        this.match(statementParser.O_CLOSEO);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function Required_sequenceContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_required_sequence;
    return this;
}

Required_sequenceContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Required_sequenceContext.prototype.constructor = Required_sequenceContext;

Required_sequenceContext.prototype.O_OPENR = function() {
    return this.getToken(statementParser.O_OPENR, 0);
};

Required_sequenceContext.prototype.sequence = function() {
    return this.getTypedRuleContext(SequenceContext,0);
};

Required_sequenceContext.prototype.O_CLOSER = function() {
    return this.getToken(statementParser.O_CLOSER, 0);
};

Required_sequenceContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterRequired_sequence(this);
	}
};

Required_sequenceContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitRequired_sequence(this);
	}
};

Required_sequenceContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitRequired_sequence(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.Required_sequenceContext = Required_sequenceContext;

statementParser.prototype.required_sequence = function() {

    var localctx = new Required_sequenceContext(this, this._ctx, this.state);
    this.enterRule(localctx, 12, statementParser.RULE_required_sequence);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 50;
        this.match(statementParser.O_OPENR);
        this.state = 51;
        this.sequence();
        this.state = 52;
        this.match(statementParser.O_CLOSER);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function Optional_repeated_sequenceContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_optional_repeated_sequence;
    return this;
}

Optional_repeated_sequenceContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Optional_repeated_sequenceContext.prototype.constructor = Optional_repeated_sequenceContext;

Optional_repeated_sequenceContext.prototype.O_OPENO = function() {
    return this.getToken(statementParser.O_OPENO, 0);
};

Optional_repeated_sequenceContext.prototype.O_COMMA = function() {
    return this.getToken(statementParser.O_COMMA, 0);
};

Optional_repeated_sequenceContext.prototype.sequence = function() {
    return this.getTypedRuleContext(SequenceContext,0);
};

Optional_repeated_sequenceContext.prototype.O_CLOSEO = function() {
    return this.getToken(statementParser.O_CLOSEO, 0);
};

Optional_repeated_sequenceContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterOptional_repeated_sequence(this);
	}
};

Optional_repeated_sequenceContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitOptional_repeated_sequence(this);
	}
};

Optional_repeated_sequenceContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitOptional_repeated_sequence(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.Optional_repeated_sequenceContext = Optional_repeated_sequenceContext;

statementParser.prototype.optional_repeated_sequence = function() {

    var localctx = new Optional_repeated_sequenceContext(this, this._ctx, this.state);
    this.enterRule(localctx, 14, statementParser.RULE_optional_repeated_sequence);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 54;
        this.match(statementParser.O_OPENO);
        this.state = 55;
        this.match(statementParser.O_COMMA);
        this.state = 56;
        this.sequence();
        this.state = 57;
        this.match(statementParser.O_CLOSEO);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function TokenContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_token;
    return this;
}

TokenContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
TokenContext.prototype.constructor = TokenContext;

TokenContext.prototype.M_TOKEN = function() {
    return this.getToken(statementParser.M_TOKEN, 0);
};

TokenContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterToken(this);
	}
};

TokenContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitToken(this);
	}
};

TokenContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitToken(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.TokenContext = TokenContext;

statementParser.prototype.token = function() {

    var localctx = new TokenContext(this, this._ctx, this.state);
    this.enterRule(localctx, 16, statementParser.RULE_token);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 59;
        this.match(statementParser.M_TOKEN);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};

function RepeatContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = statementParser.RULE_repeat;
    return this;
}

RepeatContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
RepeatContext.prototype.constructor = RepeatContext;

RepeatContext.prototype.O_3DOT = function() {
    return this.getToken(statementParser.O_3DOT, 0);
};

RepeatContext.prototype.enterRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.enterRepeat(this);
	}
};

RepeatContext.prototype.exitRule = function(listener) {
    if(listener instanceof statementListener ) {
        listener.exitRepeat(this);
	}
};

RepeatContext.prototype.accept = function(visitor) {
    if ( visitor instanceof statementVisitor ) {
        return visitor.visitRepeat(this);
    } else {
        return visitor.visitChildren(this);
    }
};




statementParser.RepeatContext = RepeatContext;

statementParser.prototype.repeat = function() {

    var localctx = new RepeatContext(this, this._ctx, this.state);
    this.enterRule(localctx, 18, statementParser.RULE_repeat);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 61;
        this.match(statementParser.O_3DOT);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


exports.statementParser = statementParser;
