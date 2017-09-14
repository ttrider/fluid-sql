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
var JavaGenerator = (function (_super) {
    __extends(JavaGenerator, _super);
    function JavaGenerator(project) {
        return _super.call(this, project) || this;
    }
    Object.defineProperty(JavaGenerator.prototype, "lang", {
        get: function () { return "Java"; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(JavaGenerator.prototype, "ext", {
        get: function () { return "java"; },
        enumerable: true,
        configurable: true
    });
    return JavaGenerator;
}(generator_1.default));
exports.default = JavaGenerator;
//# sourceMappingURL=java.js.map