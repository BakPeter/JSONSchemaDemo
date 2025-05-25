Vue.createApp({
  data() {
    return {
      data: {
        productId: 1,
        productName: 'A green door',
        price: 12.5,
        tags: ['home', 'green'],
      },
      schema: {
        $schema: 'http://json-schema.org/draft-04/schema#',
        type: 'object',
        properties: {
          productId: {
            type: 'integer',
          },
          productName: {
            type: 'string',
          },
          price: {
            type: 'number',
          },
          tags: {
            type: 'array',
            items: {
              type: 'string',
            },
          },
        },
        required: ['productId', 'productName', 'price', 'tags'],
      },
    };
  },
  methods: {
    getJsonData() {
      return this.data;
    },
    getJsonSchema() {
      return this.schema;
    },
  },
}).mount('#app');

// const buttonEl = document.querySelector('button');
// const inputEl = document.querySelector('input');
// const listEl = document.querySelector('ul');

// function addGoal() {
//   console.log('button prassed');
// }

// buttonEl.addEventListener('click', addGoal);
// // buttonEl.addEventListener('click', () => {
// //   console.log('button prassed');
// // });
