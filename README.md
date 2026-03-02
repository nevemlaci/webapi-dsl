## Current progress

As of now you can define entites for your API. A domain model is created and this model is used by language specific generators to generate code. 

The ASP.NET generator almost works for entities and CRUD operations for said entities. Only a few validations, `using` directives and a generated example project is missing.

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

## Example

An example config and entity definitions is available [here](https://github.com/nevemlaci/webapi-dsl/blob/master/WebAPI_DSL_Main/test.restapi).
