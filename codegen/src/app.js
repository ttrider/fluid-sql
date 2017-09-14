"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var path = require("path");
var async = require("async");
var fs = require("fs");
var argv = require('minimist')(process.argv.slice(2));
var project_1 = require("./project");
var yaml = require("js-yaml");
var antlr4 = require("antlr4");
var statementLexer = require("./grammar/statementLexer");
var statementParser = require("./grammar/statementParser");
var statementListener = require("./grammar/statementListener");
var statementProcessor = require("./grammar/statementProcessor");
//     var txt = "BEGIN { TRAN | TRANSACTION }   [ { transaction_name | @tran_name_variable } [ WITH MARK [ 'description' ] ] ]";
//    var chars = new antlr4.InputStream(txt);
//    var lexer = new statementLexer.statementLexer(chars);
//    var tokens  = new antlr4.CommonTokenStream(lexer);
//    var parser = new statementParser.statementParser(tokens);
//    parser.buildParseTrees = true;
//    var tree = parser.statement();
//    var l = new statementListener.statementListener();
// import mdproject = require("./mdProject");
// import cs = require("./csGenerator");
// import xsd = require("./xsdGenerator");
// import xml = require("./xmlGenerator");
// import yuml = require("./yumlGenerator");
if (argv.h || argv.help) {
    printUsage();
}
var input = getPath(argv.i);
if (!input) {
    console.error("no input file specified");
    printUsage();
}
input = path.resolve(input);
console.log("reading " + input);
fs.readFile(input, function (err, content) {
    if (!err) {
        try {
            project_1.default.statements = yaml.safeLoad(content);
            var statement = project_1.default.statements[0];
            var s = statement.beginTransaction.dialects[0];
            //project.sql = JSON.parse(content);
            project_1.default.projectDir = path.dirname(input);
        }
        catch (e) {
            console.error(e);
        }
        console.log("processing " + input);
        project_1.default.process(function () {
            process.exit(0);
        });
    }
});
function printUsage() {
    console.log("usage:");
    console.log("  -i <input mdproj.json file>");
    process.exit(1);
}
function getPath(path1, path2, altExt) {
    var p = coalesce(path1, path2);
    if (!p) {
        return null;
    }
    p = path.normalize(p);
    if (!altExt) {
        return p;
    }
    var parts = path.parse(p);
    return path.join(parts.dir, parts.name + altExt);
}
function coalesce(val1, val2, val3) {
    if (val2 === void 0) { val2 = null; }
    if (val3 === void 0) { val3 = null; }
    if (!val1) {
        if (!val2) {
            if (!val3) {
                return null;
            }
            return val3;
        }
        return val2;
    }
    return val1;
}
//# sourceMappingURL=app.js.map