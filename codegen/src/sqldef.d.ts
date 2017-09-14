
interface SQLGenerator {

    generate(done: () => void);

}

interface SQLProject {
    projectDir: string;
    sql: SQLDefinition;
}

interface SQLDefinition {
    frameworks: { [name: string]: SQLFramework; };
    dialects: { [name: string]: SQLDialect; };
    statements: { [name: string]: SQLStatement; };
    tokens: { [name: string]: SQLToken; };
}

interface SQLDialect {
    name?: string;
}

interface SQLFramework {
    namespace?: string;
    imports: SQLFrameworkImport[]
}

interface SQLFrameworkImport {
    import: string;
    into?: string;
}

interface SQLStatement {
    name?: string;
}

interface SQLToken {
    name?: string;
}