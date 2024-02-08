import React, { useEffect, useState } from "react";
import axios from "axios";
import { useDispatch, useSelector } from "react-redux";
import "../ProductsPage.css"; // Import the CSS file
import Notification from "../Components/Notification";
import PayPalButton from "../Components/PaypalButton";
import { PayPalScriptProvider } from "@paypal/react-paypal-js";

const ProductsPage = () => {
  const dispatch = useDispatch();
  const userId = useSelector((state) => state.userId);
  const [products, setProducts] = useState([]);
  const [cartItems, setCartItems] = useState({});
  const [totalPrice, setTotalPrice] = useState(0);
  const [paymentMethod, setPaymentMethod] = useState(0); // Default to COD (Cash on Delivery)
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await axios.get(
          "http://localhost:8717/api/Product/GetAll",
          {
            userId,
          }
        );

        if (response.status === 200) {
          const data = response.data;
          dispatch({ type: "SET_PRODUCTS", payload: data });
          setProducts(data);
        } else {
          // Handle error
          console.error("Failed to fetch products");
        }
      } catch (error) {
        console.error("Failed to fetch products", error);
      }
    };

    fetchProducts();
  }, [dispatch, userId]);

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const response = await axios.get(
          "http://localhost:8717/api/Order/GetAll",
          {
            headers: {
              userId,
            },
          }
        );

        if (response.status === 200) {
          const data = response.data;
          if (data.length > 0) {
            setOrders(data);
          }
        } else {
          // Handle error
          console.error("Failed to fetch orders");
        }
      } catch (error) {
        console.error("Failed to fetch orders", error);
      }
    };

    fetchOrders();
  }, [userId]);

  useEffect(() => {
    const cartItemList = Object.entries(cartItems).filter(
      ([_, quantity]) => quantity > 0
    );
    const totalPrice = cartItemList.reduce((total, [productId, quantity]) => {
      const product = products.find((product) => product.id === productId);
      return total + (product ? product.price * quantity : 0);
    }, 0);
    setTotalPrice(totalPrice);
  }, [cartItems, products]);

  const addToCart = (productId, quantity) => {
    setCartItems((prevCartItems) => ({
      ...prevCartItems,
      [productId]: (prevCartItems[productId] || 0) + quantity,
    }));
  };

  const handleOrder = async () => {
    try {
      const orderData = {
        userId,
        orderId: userId, // You can generate a unique order id here
        products: cartItems,
        orderType: paymentMethod, // Set the order type based on the selected payment method
        orderStatus: 0,
        totalPrice,
      };
      const response = await axios.post(
        "http://localhost:8717/api/Order/Create",
        orderData
      );
      if (response.status === 200) {
        // Handle successful order creation
        console.log("Order created successfully");
      } else {
        // Handle error
        console.error("Failed to create order");
      }
    } catch (error) {
      console.error("Failed to create order", error);
    }
  };

  const handlePay = async (orderId, orderType) => {
    try {
      const response = await axios.post(
        "http://localhost:8717/api/Order/Pay",
        null,
        {
          headers: {
            orderId: orderId,
            orderType: orderType,
          },
        }
      );
      if (response.status === 200) {
        // Handle successful payment
        console.log("Order paid successfully");
        // Update the order status locally
        setOrders((prevOrders) =>
          prevOrders.map((order) =>
            order.orderId === orderId ? { ...order, orderStatus: 1 } : order
          )
        );
      } else {
        // Handle error
        console.error("Failed to pay for order");
      }
    } catch (error) {
      console.error("Failed to pay for order", error);
    }
  };

  return (
    <PayPalScriptProvider
      options={{
        "client-id":
          "Af6ZvGiblPSqs--B40xk2F7xFijylREgGhECvCDMywOXBHX1ylnWZJCMyDsnZfe87qe0Wg6TN2wHz_-2",
      }}
    >
      <div className="products-container">
        <Notification />
        <h2>Products</h2>
        <div className="products-list">
          {products.map((product) => (
            <div key={product.id} className="product-item">
              <h3>{product.name}</h3>
              <p>
                <strong>Description:</strong> {product.description}
              </p>
              <p>
                <strong>Category:</strong> {product.category}
              </p>
              <p>
                <strong>Price:</strong> ${product.price.toFixed(2)}
              </p>
              <div className="add-to-cart">
                <label htmlFor={`quantity-${product.id}`}>Quantity:</label>
                <input
                  type="number"
                  id={`quantity-${product.id}`}
                  value={cartItems[product.id] || ""}
                  onChange={(e) =>
                    setCartItems((prevCartItems) => ({
                      ...prevCartItems,
                      [product.id]: parseInt(e.target.value) || 0,
                    }))
                  }
                />
                <button
                  onClick={() =>
                    addToCart(product.id, cartItems[product.id] || 0)
                  }
                >
                  Add to Cart
                </button>
              </div>
            </div>
          ))}
        </div>
        <div className="cart-container">
          <h2>Cart</h2>
          <ul>
            {Object.entries(cartItems).map(([productId, quantity]) => (
              <li key={productId}>
                {products.find((product) => product.id === productId)?.name}:{" "}
                {quantity}
              </li>
            ))}
          </ul>
          <p>
            <strong>Total Price:</strong> ${totalPrice.toFixed(2)}
          </p>
          <div className="payment-method">
            <label htmlFor="payment-method">Payment Method:</label>
            <select
              id="payment-method"
              value={paymentMethod}
              onChange={(e) => setPaymentMethod(parseInt(e.target.value))}
            >
              <option value={0}>Cash on Delivery (COD)</option>
              <option value={1}>Paypal</option>
            </select>
          </div>
          <button onClick={handleOrder}>Order</button>
        </div>
        <div className="orders-container">
          <h2>Orders</h2>
          <ul>
            {orders.map((order) => (
              <li
                key={order.orderId}
                className={order.orderStatus === 1 ? "paid-order" : ""}
              >
                <p>
                  <strong>Order ID:</strong> {order.orderId}
                </p>
                <p>
                  <strong>Total Price:</strong> ${order.totalPrice.toFixed(2)}
                </p>
                <div>
                  <button onClick={() => handlePay(order.orderId, 0)}>
                    Pay COD
                  </button>
                  <PayPalButton
                    orderId={order.orderId}
                    totalPrice={order.totalPrice}
                  ></PayPalButton>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </PayPalScriptProvider>
  );
};

export default ProductsPage;
