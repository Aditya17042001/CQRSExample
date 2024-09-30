import React, { useState, useEffect } from 'react';
import axios from 'axios';

function App() {
  const [events, setEvents] = useState([]);
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');

  // Fetch events from the backend (read model)
  const fetchEvents = async () => {
    const response = await axios.get('http://localhost:5284/api/queries/users');
    setEvents(response.data);
  };

  // Handle adding a new user (command)
  const addUser = async () => {
    await axios.post('http://localhost:5284/api/commands/add-user', {
      id: Math.random().toString(36).substr(2, 9),
      name,
      email,
    });
    setName('');
    setEmail('');
    fetchEvents(); // Refresh events
  };

  useEffect(() => {
    fetchEvents();
  }, []);

  return (
    <div className="App">
      <h1>CQRS with React and .NET WebAPI</h1>
      
      <div>
        <input 
          type="text" 
          placeholder="Name" 
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <input 
          type="text" 
          placeholder="Email" 
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <button onClick={addUser}>Add User</button>
      </div>
      
      <h2>Users</h2>
      <ul>
        {events.map(event => (
          <li key={event.id}>{event.name} - {event.email}</li>
        ))}
      </ul>
    </div>
  );
}

export default App;
