"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var path = require("path");
var async = require("async");
var fs = require("fs");
var mkdirp = require("mkdirp");
var cw = require("node-code-writer");
var Generator = (function () {
    function Generator(project) {
        this.project = project;
    }
    Object.defineProperty(Generator.prototype, "lang", {
        get: function () { return null; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Generator.prototype, "ext", {
        get: function () { return null; },
        enumerable: true,
        configurable: true
    });
    Generator.prototype.generate = function (end) {
        var _this = this;
        console.log("generating " + this.lang + " code");
        console.log("projectDir '" + this.project.projectDir + "'");
        var outdir = path.normalize(path.join(this.project.projectDir, "..", "lib", this.ext));
        mkdirp.sync(outdir);
        var statementsOutdir = path.join(outdir, "statements");
        mkdirp.sync(statementsOutdir);
        // generate statements files
        async.eachSeries(this.project.sql.statements, function (statement, cb) {
            var outFile = path.join(statementsOutdir, statement.name + "Statement." + _this.ext);
            console.log(outFile);
            var outputFile = fs.WriteStream(outFile);
            var w = new cw.CodeWriter(outputFile);
            _this.writeHeader(w);
            _this.writeImports(w);
            if (_this.writeNamespace(w)) {
                _this.writeStatement(w, statement);
                w.writeClose();
            }
            else {
                _this.writeStatement(w, statement);
            }
            outputFile.end(function () { return cb(); });
        }, function () {
            console.log("done");
            end();
        });
    };
    Generator.prototype.writeHeader = function (w) {
        w.writeLine('// <license>');
        w.writeLine('//     The MIT License (MIT)');
        w.writeLine('// </license>');
        w.writeLine('// <copyright company="TTRider Technologies.">');
        w.writeLine('//     Copyright (c) 2014-2017 All Rights Reserved');
        w.writeLine('// </copyright>');
        w.writeLine();
    };
    Generator.prototype.writeImports = function (w) {
    };
    Generator.prototype.writeNamespace = function (w) {
        return false;
    };
    Generator.prototype.writeStatement = function (w, statement) {
    };
    return Generator;
}());
exports.default = Generator;
//# sourceMappingURL=generator.js.map