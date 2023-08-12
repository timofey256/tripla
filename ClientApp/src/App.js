import React from 'react';
import { useHistory } from 'react-router-dom';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import FlightForm from './FlightForm';
import ResponsePage from './ResponsePage';
import axios from 'axios';
import './styles.css';

function App() {
  const handleSearchResults = (data) => {
    // Handle the retrieved flight data here
    console.log(data);
  };

  return (
  <Router>
      <Routes>
        <Route exact path="/" element={<FlightForm/>} />
        <Route exact path="/response" element={<ResponsePage/>} />
      </Routes>
    </Router>
  );
}

export default App;

