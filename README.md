# Infrastructure C# 

## Flowchart of the project

![Flowchart](https://github.com/user-attachments/assets/bf2b2228-2324-4e36-aa33-80580c17121e)

---

### Links to documentation of parts

---

#### [WebAPI Documentation and Services](https://github.com/MarioIliescu/SEP3-CSharp/blob/master/WebAPI_Docs.md)

#### [GrpcAPI and GrpcHandlers Documentation](https://github.com/MarioIliescu/SEP3-CSharp/blob/master/GrpcAPI_DOCS.md)

#### [Java Server](https://github.com/MarioIliescu/Sep3-Java)

#### [Blazor Client Documentation](https://github.com/MarioIliescu/SEP3-CSharp/blob/master/BlazorClientDocs.md)

---

### General Contracts

Contracts that are followed by the application (Interfaces)

---

#### API Contracts

##### Dtos

Records have been used to transfer data between Api's
In this way the client gets only the data it is required to complete the action and not the full entity.  

`Request`  
Mainly used to send data between backend servers.  

```C#
using ApiContracts.Enums;

namespace ApiContracts;

public record Request(ActionType Action, HandlerType Handler, object Payload);
```

`Response`  
For server status errors and receiving the needed payload.  

```C#
using ApiContracts.Enums;

namespace ApiContracts;

public record Response(Status Status, object Payload);
```

Example of other dtos.  
CompanyDto, used to send `Company` entity needed data to the client.

```C#
namespace ApiContracts;

public record CompanyDto(string McNumber, string CompanyName);
```

---

#### Enums

Enums are used to mark specific actions in the code.  
There are mainly 3 types of enums that are used in the app.

##### ActionType

Used to specify which action is needed to be taken by the service.

```C#
namespace ApiContracts.Enums;
public enum ActionType
{
    Unknown,
    Create,
    Get,
    Update,
    Delete,
    List,
}
```

##### HandlerType

The handler that will be used for the action, switching the implementation of the handler depending on the `Request`.

```C#
namespace ApiContracts.Enums;

public enum HandlerType
{
    Unknown,
    Company,
    //More in the future
}
```

##### Status

Used to get the status of the operation

```C#
public enum Status
{
    Ok,
    Error,
    NotFound,
    Unauthorized,
    InvalidRequest,
}
```

---

#### Entities

Are used for the model of the app, they have the basic existance criteria.  
Ex : McNumber for a company must be 10 characters long.

Company entity example

```C#
namespace Entities;
//Builder design has been used to force the usage of the set methods.
public class Company
{
    public string McNumber { get; private set; }
    public string CompanyName { get; private set; }
    public int Id { get; set; } = 0;

    private Company() { } // private constructor, only builder can create

    // Builder inner class
    public class Builder
    {
        private string _mcNumber = "DEFAULTVAL";
        private string _companyName = "Default Name";
        private int _id = 0;

        public Builder SetMcNumber(string mcNumber)
        {
            if (string.IsNullOrEmpty(mcNumber))
                throw new ArgumentException("McNumber cannot be null or empty");
            if (mcNumber.Length != 10)
                throw new ArgumentException("McNumber must be 10 characters long");
            _mcNumber = mcNumber;
            return this; // fluent
        }

        public Builder SetCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
                throw new ArgumentException("CompanyName cannot be null or empty");
            _companyName = companyName;
            return this;
        }

        public Builder SetId(int id)
        {
            if (id < 0) 
                throw new ArgumentException("Id cannot be negative");
            _id = id;
            return this;
        }

        public Company Build()
        {
            return new Company
            {
                McNumber = _mcNumber,
                CompanyName = _companyName,
                Id = _id
            };
        }
    }
}
```

#### Persistance  contract  

Used to `Handle` Requests needed for persistance.  
Implementation can be changed between Database and memory.  
Takes a `Request` and returns a `Task` with an `object`

```C#
﻿namespace PersistanceContracts;
using ApiContracts;
public interface IFleetPersistanceHandler
{
    Task<object> HandleAsync(Request request);
}
```
