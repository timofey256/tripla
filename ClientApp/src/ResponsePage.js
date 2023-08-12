import React from 'react';
import { useLocation } from 'react-router-dom';

const ResponsePage = () => {
    const location = useLocation();
    const responseData = location.state?.responseData;
  
    return (
      <div>
        <h2>Response Page</h2>
        <pre>{JSON.stringify(responseData, null, 2)}</pre>
      </div>
    );
  };

export default ResponsePage;
