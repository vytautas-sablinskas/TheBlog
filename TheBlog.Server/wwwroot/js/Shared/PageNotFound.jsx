import React from 'react';

const PageNotFound = () => {
    return (
        <div className='d-flex flex-column justify-content-center align-items-center vh-100'>
            <img src='/images/404.png' className='img-fluid' alt='404' style={{ width: '250px' }} />
            <h1>Page not found.</h1>
        </div>
    );
}

export default PageNotFound;