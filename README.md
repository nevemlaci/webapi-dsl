## Current progress

As of now you can define entites for your API. A domain model is created and this model is used by language specific generators to generate code. 

The ASP.NET generator works for entities and CRUD operations for said entities.

## Syntax

### Config

The config is a hashmap. You can put any key-value pairs here and they will be available
in the `DomainModel` that is constructed by the language processor.

### Entitites

Entities are your usual Web API entities. They can have fields of different types.

> Primitive types are: `int`, `double`, `bool` and `string`

Fields can also have types of other entities, these fields are called relations.

Each entity will have an API endpoint generated with basic CRUD operations.

Entities are defined using the `entity` keyword using a syntax inspired by
trailing type annotation languages like Kotlin and TypeScript.

```
entity User{
    username : string
    age : int
}
```

The language doesn't use semicolons to spearate fields.

Entities can have `List<T>` members to either have a list of primitives,
or a many-to-X relationship with another entity.

### Annotations

Annotations attach extra metadata to entities or fields. Annotations start with an `@` character
followed by the name of the annotations and an optional parameter list. Parameters are separated by commas, allowing
trailing commas and are always named. If a name isn't given to a parameter, it will have an implicit name determined
by the index of the parameter

```
@Route("users") //implicit parameter name __arg1
@Route(route="users") //explicit parameter name
entity User{
    @Unique
    username : string
    age : int
}
```

Unnamed parameters are not allowed after named parameters.

## Example Project
An example of using the generators in a project is shown in `WebAPI_DSL_TestingProject`.

To run the test project with codegen:
```bash
dotnet run --project ./WebAPI_DSL_Main/WebAPI_DSL_Main.csproj -- -i "./WebAPI_DSL_TestingProject/test.restapi" -o "./WebAPI_DSL_TestingProject" -g "aspnet"
dotnet run --project ./WebAPI_DSL_TestingProject/WebAPI_DSL_TestingProject.csproj
```

Make sure the correct `using` directives are included in the source files.
