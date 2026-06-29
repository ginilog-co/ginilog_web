// app/api/proxy/bookings/route.ts
import { NextRequest, NextResponse } from 'next/server';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const reservationId = request.headers.get('reservationId') || '';
    const authHeader = request.headers.get('authorization') || '';
    
    console.log('🔵 ====== PROXY REQUEST ======');
    console.log('Reservation ID:', reservationId);
    console.log('Auth Header:', authHeader ? 'Present' : 'Missing');
    console.log('Body:', JSON.stringify(body, null, 2));

    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    
    if (reservationId) {
      headers['reservationId'] = reservationId;
    }
    
    if (authHeader) {
      headers['Authorization'] = authHeader;
    }

    console.log('🔵 Headers being sent:', Object.keys(headers));

    const response = await fetch(
      'https://api-data-connection.ginilog.org/api/bookings/accomodation',
      {
        method: 'POST',
        headers,
        body: JSON.stringify(body),
      }
    );

    const data = await response.text();
    
    console.log('🟢 ====== PROXY RESPONSE ======');
    console.log('Status:', response.status);
    console.log('Body:', data);

    return new NextResponse(data, {
      status: response.status,
      headers: {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
        'Access-Control-Allow-Headers': 'Content-Type, Authorization, reservationId',
      },
    });
  } catch (error) {
    console.error('🔴 Proxy error:', error);
    return NextResponse.json(
      { error: 'Proxy error: ' + (error instanceof Error ? error.message : 'Unknown error') },
      { status: 500 }
    );
  }
}

export async function OPTIONS() {
  return new NextResponse(null, {
    status: 204,
    headers: {
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
      'Access-Control-Allow-Headers': 'Content-Type, Authorization, reservationId',
    },
  });
}