"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var TSGenerator = (function () {
    function TSGenerator() {
    }
    TSGenerator.prototype.generate = function (project, done) {
        console.log("generating NodeJS TypeScript code");
        done();
    };
    return TSGenerator;
}());
var tsGenerator = new TSGenerator();
exports.default = tsGenerator;
//# sourceMappingURL=ts.js.map