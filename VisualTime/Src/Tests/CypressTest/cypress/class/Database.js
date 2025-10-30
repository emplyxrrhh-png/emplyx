const { t } = require('i18next-ko');
const sql = require('mssql');

const config = {
  user: 'saDev',
  password: 'VisualTime#1',
  server: '10.9.0.4',
  database: 'devDev_cypress',
options: {
  //encrypt: true,
  trustServerCertificate: true,
},
};

class Database {

  constructor() {
    console.log("hdhdhdhdhdh")
    //this._poolPromise = new sql(config).connect();
  }

  async query(queryString) {

    try {
   
      await sql.connect(config) 
      //sql.connect('Application Name=VisualTime Live Business;Data Source=10.9.0.4;Initial Catalog=devDev_cypress;User ID=saDev;Password=VisualTime#1') //;Encrypt=true
      const result =  await sql.query("INSERT INTO sysroLiveAdvancedParameters VALUES ('VICTOR TEST','VAMOSSS!!!')")
      console.log(result)
        console.log("hdhdhdhdhdh")
    } catch (err) {
      console.error('Error en la consulta SQL: ', err);
    } finally {
      console.log('Promesa completada');
    };
  }
  // Otras funciones útiles para manejar transacciones, procedimientos almacenados, etc.
}

module.exports = Database;