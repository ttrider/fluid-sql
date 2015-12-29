fluid-sql
=========


Release Notes
==
1.5.4
--
* Adding overlooked unary minus token support
* Bug Fixes
1.5.0
--
* Initial support for SCHEMA
* Bug Fixes
1.4.1
--
* Name token improvements
* CTE improvements
* Bug Fixes
1.3.0
--
* Bug Fixes
1.2.0
--
* Adding more Parameter-related helper functions, like Sql.Parameter.XXX
1.1.1
--
* Adding Parameter direction support

1.1.0
--
* Renaming parameterized snippet to Templated Statement
* Adopting semantic versioning

1.0.21
--
* Adding parameterized snippet support


0.0.23.1
--
* Support for CREATE/ALTER/DROP VIEW

0.0.22.1
--
* Support for WHILE, BREAK AND CONTINUE
* Support for GOTO
* Support for TRY/CATCH
* bug fixes

0.0.21.2
--
* Support for MERGE statement
* Debug-time visualizer
* bug fixes

0.0.20.0
--
* Support for UPDATE statement
* bug fixes

0.0.19.0
--
* Support for SET @variable = value
* Support for SELECT @variable = value
* bug fixes

0.0.18.0
--
* Support for TABLE variable
* bug fixes


0.0.17.0
--
* Support for HAVING
* bug fixes
==
0.0.16.0
--
* Support for EXISTS and NOT EXISTS
* bug fixes
0.0.15.3
--
* Fixing Create Table IndexOn.
* Bug fixes

0.0.14.0
--
* Support for DELETE with JOIN

0.0.12.1
--
* Support for temporary tables

0.0.11.2
--
* Ability to create command that is not connected yet
* Support for Snipper as a statement with potential output
* bug fixes

0.0.10.0
--
* Ability to create command that is not connected yet

0.0.9
--
* Support for CREATE TABLE
* Support for taoke or statement stringify
* Bug fixes

0.0.8
--
* Support for CREATE/ALTER/DROP INDEX

0.0.7
--
* Support for comments
* Support for INSERT statement

0.0.6.2
--
* Ability to provide parameter values during statement construction
* bug fixes

0.0.5
--
* Support for parameters with default values
* Bug fix for the empty part in Name ( like [tempdb]..[name])

0.0.4
--
* Support for WrapAsSelect statement

0.0.3
--
* Support for DELETE FROM
* Support for LIKE

0.0.2
--
* Support for IEnumerable parameters complementing params[]
* Support for DROP TABLE statement
* Support for parameterized Snippets
