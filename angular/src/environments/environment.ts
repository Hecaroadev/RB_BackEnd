 import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44351/',
  redirectUri: baseUrl,
  clientId: 'RoomBooking_App',
  responseType: 'code',
  scope: 'offline_access RoomBooking',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'RoomBooking',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44351',
      rootNamespace: 'Mila.RoomBooking',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
