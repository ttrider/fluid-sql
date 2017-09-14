"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var async = require("async");
var cs_1 = require("./generators/cs");
var java_1 = require("./generators/java");
var node_1 = require("./generators/node");
var Project = (function () {
    function Project() {
    }
    Project.prototype.process = function (done) {
        // preprocess
        for (var name_1 in this.sql.statements) {
            this.sql.statements[name_1].name = name_1;
        }
        for (var name_2 in this.sql.tokens) {
            this.sql.tokens[name_2].name = name_2;
        }
        for (var name_3 in this.sql.dialects) {
            this.sql.dialects[name_3].name = name_3;
        }
        var csg = new cs_1.default(this);
        var javag = new java_1.default(this);
        var nodeg = new node_1.default(this);
        async.eachSeries([csg, javag, nodeg], function (generator, cb) {
            generator.generate(function () {
                cb(null);
            });
        }, function () {
            console.log("done");
            process.exit(0);
        });
    };
    return Project;
}());
var project = new Project();
exports.default = project;
//# sourceMappingURL=project.js.map