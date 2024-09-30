import React, { useState } from 'react';

const UserForm = ({  }) => {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');

    const submitForm = async () => {
        await fetch('http://localhost:5284/api/users', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, email }),
        });
       
    };

    return (
        <div>
            <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Name" />
            <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
            <button onClick={submitForm}>Add User</button>
        </div>
    );
};

export default UserForm;
