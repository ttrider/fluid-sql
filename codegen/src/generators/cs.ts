import path = require("path");
import async = require("async");
import fs = require("fs");
import mkdirp = require("mkdirp");
import cw = require("node-code-writer");
import Generator from "./generator";


class CSGenerator extends Generator implements SQLGenerator {

    constructor(project: SQLProject){
        super(project);
    }

    get lang(): string { return "C#"; }
    get ext(): string { return "cs"; }


    writeImports(w: cw.CodeWriter) {

        var fw = this.project.sql.frameworks["cs"];
        if (fw && fw.imports) {
            for (let i = 0; i < fw.imports.length; i++) {
                var im = fw.imports[i];
                if (im) {
                    w.write('using');
                    if (im.into) {
                        w.write(' ');
                        w.write(im.into);
                        w.write(' = ');
                    }
                    w.write(' ');
                    w.write(im.import);
                    w.writeLine(';');
                }
            }

        }
        w.writeLine();
    }

    writeNamespace(w: cw.CodeWriter): boolean {

        var fw = this.project.sql.frameworks["cs"];
        if (fw && fw.namespace) {
            w.write('namespace');
            w.write(' ');
            w.writeLine(fw.namespace);
            w.writeOpen();
            return true;
        }
        return false;
    }

}

export default CSGenerator;