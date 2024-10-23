import { Router, Request, Response } from 'express';

const manageRouter = Router();

/**
 * @swagger
 * /manage/health:
 *   get:
 *     summary: Check service health
 *     description: Returns the status of the service.
 *     responses:
 *       200:
 *         description: Service is up and running.
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 status:
 *                   type: string
 *                   example: OK
 *                 uptime:
 *                   type: number
 *                   example: 1234
 *                 timestamp:
 *                   type: string
 *                   example: 2024-10-23T09:37:10.349Z
 */
manageRouter.get('/health', (req: Request, res: Response) => {
  res.status(200).json({
    status: 'OK',
    uptime: process.uptime(),
    timestamp: new Date().toISOString(),
  });
});

export default manageRouter;