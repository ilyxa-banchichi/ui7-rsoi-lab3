import express from 'express';
import { setupSwagger } from './swagger';
import manageRouter from './routes/manage';

const app = express();
const PORT = 80;

app.use(express.json());
setupSwagger(app);

app.use("/manage", manageRouter);

app.listen(PORT, () => {
  console.log(`ReservationService running on port ${PORT}`);
});