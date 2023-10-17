const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:56813';

  const PROXY_CONFIG = [

    {
      context: [
        "/user/",
        "/deviceconfig/",
        "/attendancelog/",
        "/account/",
     ],
      target: target,
      secure: false,
      headers: {
        Connection: 'Keep-Alive'
    }
    // },
    // {
    //   context: [
    //     "/deviceconfig/",
    //  ],
    //   target: target,
    //   secure: false,
    //   headers: {
    //     Connection: 'Keep-Alive'
    //   }
    // },
    // {
    //   context: [
    //     "/attendancelog/",
    //  ],
    //   target: target,
    //   secure: false,
    //   headers: {
    //     Connection: 'Keep-Alive'
    //   }
    // },
    // {
    //   context: [
    //     "/account/",
    //  ],
    //   target: target,
    //   secure: false,
    //   headers: {
    //     Connection: 'Keep-Alive'
    //   }
    }
  ]

module.exports = PROXY_CONFIG;
