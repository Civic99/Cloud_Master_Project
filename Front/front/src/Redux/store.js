import { configureStore } from '@reduxjs/toolkit';

const initialState = {
  userId: null,
  products: []
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case 'SET_USER_ID':
      return {
        ...state,
        userId: action.payload
      };
    case 'SET_PRODUCTS':
      return {
        ...state,
        products: action.payload
      };
    default:
      return state;
  }
};

const store = configureStore({
  reducer
});

export default store;