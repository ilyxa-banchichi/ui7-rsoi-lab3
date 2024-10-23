import swaggerJsdoc from 'swagger-jsdoc';
import swaggerUi from 'swagger-ui-express';
import { Express } from 'express';

const options = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'Reservation Service API',
      version: '1.0.0',
      description: 'API for Reservation Service',
    }
  },
  apis: ['./src/routes/*.ts'],
};

const swaggerSpec = swaggerJsdoc(options);

export const setupSwagger = (app: Express) => {
  app.use('/swagger/index.html', swaggerUi.serve, swaggerUi.setup(swaggerSpec));
};