## Entity annotations

### @Route

Allows the user to change the route of the endpoint.

Arguments:
* `route [1]` : `string` | A string literal used as the endpoint route.

### @NoDefaultEndpoint

Disables the default endpoint for an entity.

Takes no arguments.

## Field annotations

### @Unique

Adds a unique constraint to a field.

Takes no arguments.

### @Required

Marks a field required for create operations.

Takes no arguments.

### @Filter

Turns on filtering for this field.
This will enable filtering throug a `search` endpoint with query parameters.

Arguments:

* `type` : `FilterType` | The type of the query.

#### FilterType

`FilterType` is a built in enum with values `Search` and `Range`.
A search query will look for equality in the field while a range query will generate min and max parameters.