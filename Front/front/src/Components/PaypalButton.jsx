// PayPalButton.js
import React from "react";
import { PayPalButtons, usePayPalScriptReducer } from "@paypal/react-paypal-js";
import axios from "axios";

function PayPalButton({ totalPrice, orderId }) {
  const [{ isLoaded, options }] = usePayPalScriptReducer();

  const createOrder = (data, actions) => {
    return actions.order.create({
      purchase_units: [
        {
          amount: {
            value: totalPrice, // Use the totalPrice prop here
          },
        },
      ],
    });
  };

  const onApprove = async (data, actions) => {
    // Capture the funds from the transaction
    const details = await actions.order.capture();
    // Call your backend to save transaction details
    try {
      const response = await axios.post(
        "http://localhost:8717/api/Order/Pay",
        null,
        {
          headers: {
            orderId: orderId,
            orderType: "paypal",
          },
        }
      );
      console.log("Transaction completed by " + details.payer.name.given_name);
      // Show a thank-you message to the buyer
    } catch (error) {
      console.error("Error occurred while processing payment:", error);
      // Handle error
    }
  };

  return (
    <PayPalButtons
      style={{ layout: "horizontal" }}
      createOrder={createOrder}
      onApprove={onApprove}
      options={options}
    />
  );
}

export default PayPalButton;
