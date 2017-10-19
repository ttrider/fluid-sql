"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var generator_1 = require("./generator");
var CSGenerator = (function (_super) {
    __extends(CSGenerator, _super);
    function CSGenerator(project) {
        return _super.call(this, project) || this;
    }
    Object.defineProperty(CSGenerator.prototype, "lang", {
        get: function () { return "C#"; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CSGenerator.prototype, "ext", {
        get: function () { return "cs"; },
        enumerable: true,
        configurable: true
    });
    CSGenerator.prototype.writeImports = function (w) {
        var fw = this.project.sql.frameworks["cs"];
        if (fw && fw.imports) {
            for (var i = 0; i < fw.imports.length; i++) {
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
    };
    CSGenerator.prototype.writeNamespace = function (w) {
        var fw = this.project.sql.frameworks["cs"];
        if (fw && fw.namespace) {
            w.write('namespace');
            w.write(' ');
            w.writeLine(fw.namespace);
            w.writeOpen();
            return true;
        }
        return false;
    };
    return CSGenerator;
}(generator_1.default));
exports.default = CSGenerator;
//# sourceMappingURL=cs.js.map