## Repro for ServiceStack RequestContext.Items + async problem

### The problem

During request processing, an async operation is performed. After awaiting completion, the `RequestContext.Items` accesses items that were modified by other requests.

In this project there are 2 tests:
* `Each_Request_Has_Own_Items_V1` - passes, because the items are accessed before async operation
* `Each_Request_Has_Own_Items_V2` - fails, items are accessed after async operation
* `Each_Request_Has_Own_Items_V3` - passes, uses `CallContext.LogicalSetData` instead of items
