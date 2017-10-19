var path = require("path");
var async = require("async");
var fs = require("fs");
var mkdirp = require("mkdirp");
import cw = require("node-code-writer");

export default class Generator implements SQLGenerator {

    get lang(): string { return null; }
    get ext(): string { return null; }

    constructor(public project: SQLProject){
    }

    generate(end: () => void) {


        console.log(`generating ${this.lang} code`);
        console.log(`projectDir '${this.project.projectDir}'`);
        

        let outdir = path.normalize(path.join(this.project.projectDir, "..", "lib", this.ext));
        mkdirp.sync(outdir);

        let statementsOutdir = path.join(outdir, "statements");
        mkdirp.sync(statementsOutdir);

        // generate statements files
        async.eachSeries(this.project.sql.statements, (statement, cb) => {
            let outFile = path.join(statementsOutdir, statement.name + "Statement." + this.ext);
            console.log(outFile);

            const outputFile = fs.WriteStream(outFile);
            const w = new cw.CodeWriter(outputFile);
            this.writeHeader(w);
            this.writeImports(w);
            if (this.writeNamespace(w)) {
                this.writeStatement(w, statement);
                w.writeClose();
            }
            else {
                this.writeStatement(w, statement);
            }

            outputFile.end(() => cb());

        }, () => {
            console.log("done");
            end();
        });
    }

    writeHeader(w: cw.CodeWriter) {
        w.writeLine('// <license>');
        w.writeLine('//     The MIT License (MIT)');
        w.writeLine('// </license>');
        w.writeLine('// <copyright company="TTRider Technologies.">');
        w.writeLine('//     Copyright (c) 2014-2017 All Rights Reserved');
        w.writeLine('// </copyright>');
        w.writeLine();
    }

    writeImports(w: cw.CodeWriter) {

    }

    writeNamespace(w: cw.CodeWriter): boolean {
        return false;
    }

    writeStatement(w: cw.CodeWriter, statement: SQLStatement) {

    }

}