# JSONSchemaDemo

JSON Schema is the vocabulary that enables JSON data consistency, validity, and interoperability at scale.

JSON Schema is a JSON document that describes the structure, constraints, and data types for a set of JSON data.

The instance is the JSON document that is being validated or described, and the schema is the document that contains the description.

## Table of content

- [Udemy course](#udemy-course)
- [Links](#links)
- [Usages](#usages)
  - [Streamline testing and validation](#streamline-testing-and-validation)
  - [Exchange data seamlessly](#exchange-data-seamlessly)
  - [Document your data](#document-your-data)
  - [Vibrant tooling ecosystem](#vibrant-tooling-ecosystem)
- [Concepts](#concepts)
  - [schema keywords](#schema-keywords)
  - [schema annotations](#schema-annotations)
  - [validation keyword](#validation-keyword)
- [Schema](#schema)

  - [Basic schema](#basic-schema)
  - [Create a schema definition](#create-a-schema-definition)
    - [JSON object](#json-object)
    - [Schemas](#schemas)
    - [Schema object keys](#schema-object-keys)
    - [Nested data structure](#nested-data-structure)
    - [Add external reference](#add-external-reference)

- [Validate JSON data against the schema](#validate-json-data-against-the-schema)

## Udemy course

**[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

## Links

**[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

- https://json-schema.org/
- https://vue-json-schema-form.lljj.me/
- https://form.lljj.me/v3/#/demo?ui=VueElementForm&type=Widgets

## Usages

**[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

- ### Streamline testing and validation

  Simplify your validation logic to reduce your code’s complexity and save time on development. Define constraints for your data structures to catch and prevent errors, inconsistencies, and invalid data.

- ### Exchange data seamlessly

  Establish a common language for data exchange, no matter the scale or complexity of your project. Define precise validation rules for your data structures to create shared understanding and increase interoperability across different systems and platforms.

- ### Document your data

  Create a clear, standardized representation of your data to improve understanding and collaboration among developers, stakeholders, and collaborators.

- ### Vibrant tooling ecosystem

  Adopt JSON Schema with an expansive range of community-driven tools, libraries, and frameworks across many programming languages.

## Concepts

**[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

### schema keywords

### schema annotations

### validation keyword

## Schema

- ### Basic schema

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  basic schema is a blank JSON object, which constrains nothing, allows anything, and describes nothing

  ```
  {}
  ```

- ### Create a schema definition

  #### **JSON object**:

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  ```
  {
    "productId": 1,
    "productName": "An ice sculpture",
    "price": 12.50,
    "tags": [ "cold", "ice" ],
    "dimensions": {
      "length": 7.0,
      "width": 12.0,
      "height": 9.5
    },
    "warehouseLocation": {
      "latitude": -78.75,
      "longitude": 20.4
    }
  }
  ```

  productId, productName, price are mandatory fields, rest optional

  #### **Schemas**

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  ```
  {
    "$schema": "https://json-schema.org/draft/2020-12/schema",
    "$id": "https://example.com/product.schema.json",
    "title": "Product",
    "description": "A product from Acme's catalog",
    "type": "object",
    "properties": {
      "productId": {
        "description": "The unique identifier for a product",
        "type": "integer"
      },
      "productName": {
        "description": "Name of the product",
        "type": "string"
      },
      "price": {
        "description": "The price of the product",
        "type": "number",
        "exclusiveMinimum": 0
      },
      "tags": {
        "description": "Tags for the product",
        "type": "array",
        "items": {
          "type": "string"
        },
        "minItems": 1,
        "uniqueItems": true
      },
      "dimensions": {
        "type": "object",
        "properties": {
          "length": {
            "type": "number"
          },
          "width": {
            "type": "number"
          },
          "height": {
            "type": "number"
          }
        },
        "required": [ "length", "width", "height" ]
      },
      "warehouseLocation": {
        "description": "Coordinates of the warehouse where the product is located.",
        "$ref": "https://example.com/geographical-location.schema.json"
      }
    },
    "required": [ "productId", "productName", "price" ]
  }
  ```

  ```
  https://example.com/geographical-location.schema.json:

  {
    "$id": "https://example.com/geographical-location.schema.json",
    "$schema": "https://json-schema.org/draft/2020-12/schema",
    "title": "Longitude and Latitude",
    "description": "A geographical coordinate on a planet (most commonly Earth).",
    "required": [ "latitude", "longitude" ],
    "type": "object",
    "properties": {
      "latitude": {
        "type": "number",
        "minimum": -90,
        "maximum": 90
      },
      "longitude": {
        "type": "number",
        "minimum": -180,
        "maximum": 180
      }
    }
  }
  ```

  #### **Schema object keys**

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  - **$schema**

    **schema keyword**

    specifies which draft of the JSON Schema standard the schema adheres to

  - **$id**

    **schema keyword**

    sets a unique URI for the schema, can refer element inside the schema or external JSON document

  - **title** and **description**

    **schema annotations**

    state the intent of the schema
    these keywords don’t add any constraints to the data being validated

  - **type**

    **validation keyword**

    defines the first constraint on the JSON data

  - **properties**

    **validation keyword**

    it's on object, every property in the object represents a key in the JSON data

    it's possible to defined which propeties are required

    **property keys**

    - description

      describes the property

    - type

      defines what kind of data is expected

    - exclusiveMinimu

      minimum, not included, value for type **number**

    - items

      defined the type of an item in an **array** type property

      could be also object, ref...

    - minItems

      min items for an **array** type property

    - uniqueItems

      defineds that ebery item, in an **array** type propery, is unique

  - **required**

    **validation keyword**

    defined required properties kyes

    an array of required property kew names, every other key property is optional

    ```
    "required": ["key1", "key2", ...]
    ```

    ```
    "tags": {
      "description": "Tags for the product",
      "type": "array",
      "items": {
        "type": "string"
      },
      "minItems": 1,
      "uniqueItems": true
    },
    ```

- ### Nested data structure

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  add property key of **object** type, and all of it's defenition

  ```
  {
    ...
    "dimensions": {
      "type": "object",
      "properties": {
        ...
      },
      "required: [...],
      ...
    }
  }
  ```

- ### Add external reference

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  add property key

  link external schema by setting **$ref**, a **schema keayword**, value

  add property key of **object** type, and all of it's defenition

  ```
  {
    ...
    "properties": {
      ...
      "warehouseLocation": {
        "description": "Coordinates of the warehouse where the product is located.",
        "$ref": "https://example.com/geographical-location.schema.json"
      }
    }
  }
  ```

- ### Validate JSON data against the schema

  **[`^ top ^`](#jsonschemademo)** | **[`^ Table of content ^`](#table-of-content)**

  to validate JSON data use any type of validator of yoyr choice, such as cli and browser tools, [3-rd party tools](https://json-schema.org/implementations)
