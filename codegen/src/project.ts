var async = require("async");
import cs from './generators/cs';
import java from './generators/java';
import node from './generators/node';

class Project implements SQLProject {
    process(done: () => void) {

        // preprocess
        for (let name in this.sql.statements) {
            this.sql.statements[name].name = name;
        }
        for (let name in this.sql.tokens) {
            this.sql.tokens[name].name = name;
        }
        for (let name in this.sql.dialects) {
            this.sql.dialects[name].name = name;
        }

        var csg = new cs(this);
        var javag = new java(this);
        var nodeg = new node(this);

        async.eachSeries([csg, javag, nodeg],
            (generator, cb) => {
                generator.generate(() => {
                    cb(null);
                });
            }, () => {
                console.log("done");
                process.exit(0);
            });
    }

    projectDir: string;
    sql: SQLDefinition;
    statements:any;
}

var project = new Project();
export default project;