# Guide to using Firestore Objects

## No ID necessary
(Doesn't need to know the name of the file in Firestore)

Get Document by File Name (uncommon)
```csharp
GetDocumentAsync<Type>(string collection, string documentId)
```
Get Document by Some Field's Value (Note: type cast enums to ```int``` before passing in)
```csharp
QueryDocumentsAsync<Type>(string collection, string fieldName, object value)
```
Get Document by Combination of Multiple Fields' Values (Note: type cast enums to ```int``` before passing in)
```csharp
QueryComplexDocumentsAsync<Type>(string collection, (List<QueryParams>)[
  new() {
    fieldName = "",
    value = val1
  },
  new() {
    fieldName = "",
    value = (int)enumVal2
  }
])
```
Generally intermediate items that connect two other objects:
* CartItem
* InvoiceItem
* Tag

## ID Necessary
(Need to know the name of the file in Firestore)

Assume final data type is ```Type```, in between data type is ```FirestoreType```

Get Document by Some Field's Value (Note: type cast enums to ```int``` before passing in)
```csharp
QueryDocumentsAsyncWithId<Type>(string collection, string fieldName, object value)
```
Get Document by Combination of Multiple Fields' Values (Note: type cast enums to ```int``` before passing in)
```csharp
QueryComplexDocumentsAsyncWithId<Type>(string collection, (List<QueryParams>)[
  new() {
    fieldName = "",
    value = val1
  },
  new() {
    fieldName = "",
    value = (int)enumVal2
  }
])
```
Most objects:
* Category
* Image
* Invoice
* Listing
* Log
* Order
* Request (Pending Vendor)
* User

## ID Necessary but Different Name
(Need to know the name of the file in Firestore, but it's assigned to another value, like OwnerId)

Assume final data type is ```Type```, type in database is ```FirestoreType```, and type with ID in wrong field is ```TypeWithId```

Follow above steps, then:

```csharp
Type data = Type.FromTypeWithId(TypeWithId Value);
```

* BankingInfo
* Cart