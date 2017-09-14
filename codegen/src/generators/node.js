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
var NodeGenerator = (function (_super) {
    __extends(NodeGenerator, _super);
    function NodeGenerator(project) {
        return _super.call(this, project) || this;
    }
    Object.defineProperty(NodeGenerator.prototype, "lang", {
        get: function () { return "NodeJS"; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NodeGenerator.prototype, "ext", {
        get: function () { return "ts"; },
        enumerable: true,
        configurable: true
    });
    return NodeGenerator;
}(generator_1.default));
exports.default = NodeGenerator;
//# sourceMappingURL=node.js.map