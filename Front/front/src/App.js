import logo from "./logo.svg";
import "./App.css";
import Register from "./Pages/RegisterPage";
import ProductsPage from "./Pages/ProductPage";
import PayPalScriptProvider from "@paypal/react-paypal-js";

function App() {
  return (
    <div>
      <Register />
      <ProductsPage />
    </div>
  );
}

export default App;
