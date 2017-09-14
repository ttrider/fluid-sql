import Generator from "./generator";

class NodeGenerator extends Generator implements SQLGenerator {

    constructor(project: SQLProject){
        super(project);
    }

    get lang(): string { return "NodeJS"; }
    get ext(): string { return "ts"; }

}

export default NodeGenerator;