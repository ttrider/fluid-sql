import Generator from "./generator";

class JavaGenerator extends Generator implements SQLGenerator {

    constructor(project: SQLProject){
        super(project);
    }

    get lang(): string { return "Java"; }
    get ext(): string { return "java"; }
}

export default JavaGenerator;