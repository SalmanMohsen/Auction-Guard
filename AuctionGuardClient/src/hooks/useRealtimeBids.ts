import { useEffect, useState, useRef } from 'react';
import { useSelector } from 'react-redux';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import type { RootState } from '../store/store';
import type { Bid } from '../types';
import { API_BASE_URL } from '../api/api';

export const useRealtimeBids = (auctionId: string) => {
    const [bids, setBids] = useState<Bid[]>([]);
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [error, setError] = useState<string | null>(null);
    const latestBids = useRef(bids);

    const token = useSelector((state: RootState) => state.auth.token);

    useEffect(() => {
        latestBids.current = bids;
    }, [bids]);

    useEffect(() => {
        if (!token || !auctionId) return;

        // Construct the full URL for the hub endpoint
        const hubUrl = `${API_BASE_URL.replace('/api', '')}/api/bidding-hub`;

        const newConnection = new HubConnectionBuilder()
            .withUrl(hubUrl, {
                // This function provides the JWT token for authentication
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, [auctionId, token]);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('SignalR Connection successful.');

                    // Join the specific auction group on the hub
                    connection.invoke("JoinAuctionGroup", auctionId)
                        .catch(err => console.error("Failed to join auction group: ", err));

                    // Listener for successful group join
                    connection.on("JoinSuccess", (message) => {
                        console.log(message);
                    });

                    // Listener for new bids
                    connection.on("ReceiveNewBid", (newBid: Bid) => {
                        setBids(prevBids => {
                            // Avoid adding duplicate bids
                            if (prevBids.some(b => b.bidId === newBid.bidId)) {
                                return prevBids;
                            }
                            return [...prevBids, newBid].sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
                        });
                    });

                    // Listener for bidding errors from the hub
                    connection.on("BiddingError", (errorMessage: string) => {
                        console.error("Bidding Error:", errorMessage);
                        setError(errorMessage);
                    });
                })
                .catch(err => {
                    console.error('SignalR Connection failed: ', err);
                    setError('Failed to connect to the bidding service.');
                });
        }
    }, [connection, auctionId]);

    const placeBid = async (amount: number) => {
        if (connection && connection.state === 'Connected') {
            try {
                // Invoke the PlaceBid method on the hub
                await connection.invoke("PlaceBid", auctionId, amount);
                setError(null); // Clear previous errors on successful bid
            } catch (err) {
                console.error('Failed to place bid:', err);
                setError('Failed to send bid to the server.');
            }
        } else {
            setError('You are not connected to the bidding service.');
        }
    };

    return { bids, placeBid, error, connectionStatus: connection?.state };
};
