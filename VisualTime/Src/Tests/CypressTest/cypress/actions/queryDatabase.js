// queryDatabase.js
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


(async () => {
  try {
    console.log('Promesa INICIADA');
      // make sure that any items are correctly URL encoded in the connection string
      await sql.connect(config) 
      //sql.connect('Application Name=VisualTime Live Business;Data Source=10.9.0.4;Initial Catalog=devDev_cypress;User ID=saDev;Password=VisualTime#1') //;Encrypt=true
      const result =  await sql.query("INSERT INTO sysroLiveAdvancedParameters VALUES ('VICTOR TEST','VAMOSSS!!!')")
      console.log(result)

  } catch (err) {
    console.error('Error en la consulta SQL: ', err);
  } finally {
    console.log('Promesa completada');
  };
})().then(() => {
  console.log('La función asíncrona ha terminado');
});


(async () => {
  try {
  
      // make sure that any items are correctly URL encoded in the connection string
      await sql.connect(config) 
      await sql.query("UPDATE sysroPassports_AuthenticationMethods SET InvalidAccessAttemps=0")
      await sql.query("UPDATE sysroPassports_AuthenticationMethods SET BloquedAccessApp=0")
      
      console.log(result)
  } catch (err) {
    console.error('Error en la consulta SQL: ', err);
  } finally {
    console.log('Promesa completada');
  };
})().then(() => {
  console.log('La función asíncrona ha terminado');
});


