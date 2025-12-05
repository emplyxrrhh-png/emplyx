const express = require('express');
const path = require('path');
const helmet = require('helmet');
const app = express();
const port = process.env.PORT || 3000;

app.use(helmet.hsts({
  maxAge: 5184000, // 60 días
  includeSubDomains: true,
  preload: true
}));

// Servir archivos estáticos desde la carpeta 'dist'
app.use(express.static(path.join(__dirname, 'dist')));

// Manejar cualquier otra ruta devolviendo index.html (para SPA)
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'dist', 'index.html'));
});

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
