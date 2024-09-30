// Import necessary libraries
const { EventStoreDBClient, FORWARDS, START } = require('@eventstore/db-client');
const { MongoClient } = require('mongodb');

// Configuration for EventStoreDB
const eventStoreClient = EventStoreDBClient.connectionString`esdb://localhost:2113?tls=false`;

// Configuration for MongoDB
const mongoUrl = 'mongodb://localhost:27017'; // Replace with your MongoDB connection string if necessary
const dbName = 'eventstoreDB';               // Your MongoDB database name
const collectionName = 'events';             // The collection where events will be stored

// Async function to handle the process of reading from EventStoreDB and writing to MongoDB
async function processEvents() {
  // Connect to MongoDB
  const mongoClient = new MongoClient(mongoUrl);
  await mongoClient.connect();
  const db = mongoClient.db(dbName);
  const collection = db.collection(collectionName);

  console.log('Connected to MongoDB successfully');

  // Subscribe to the EventStoreDB stream you want to read from
  const subscription = eventStoreClient.subscribeToStream('mongodb-target-stream', {
    fromRevision: START,
    direction: FORWARDS
  });

  console.log('Subscribed to EventStoreDB stream successfully');

  // Read events from the stream and insert them into MongoDB
  for await (const resolvedEvent of subscription) {
    const event = resolvedEvent.event;

    if (event) {
      console.log(`Received Event: ${event.type}`);

      // Insert the event data into MongoDB
      try {
        await collection.insertOne({
          id: event.id,
          streamId: event.streamId,
          type: event.type,
          data: event.data,
          metadata: event.metadata,
          created: event.created
        });
        console.log('Event successfully inserted into MongoDB');
      } catch (err) {
        console.error('Error inserting event into MongoDB:', err);
      }
    }
  }
}

// Start the projection service
processEvents()
  .then(() => console.log('Projection service started successfully'))
  .catch(error => console.error('Error starting projection service:', error));
