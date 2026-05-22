import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

const API_BASE_URL = 'http://localhost:5165/api'; // <--- Portul tău de C#

function App() {
  const [products, setProducts] = useState([]);
  const [warehouses, setWarehouses] = useState([]);
  const [movements, setMovements] = useState([]);

  // State pentru expandare tabel istoric
  const [isTableExpanded, setIsTableExpanded] = useState(false);

  // Formular state
  const [selectedProduct, setSelectedProduct] = useState('');
  const [fromWarehouse, setFromWarehouse] = useState('');
  const [toWarehouse, setToWarehouse] = useState('');
  const [quantity, setQuantity] = useState(0);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const prodRes = await axios.get(`${API_BASE_URL}/Products`);
      const wareRes = await axios.get(`${API_BASE_URL}/Warehouses`);
      const moveRes = await axios.get(`${API_BASE_URL}/Movements`);
      
      setProducts(prodRes.data);
      setWarehouses(wareRes.data);
      setMovements(moveRes.data);
    } catch (error) {
      console.error("Eroare la încărcarea datelor:", error);
    }
  };

  const getStockInWarehouse = (productId, warehouseId) => {
    const product = products.find(p => p.id === productId);
    let stock = warehouseId === 1 && product ? product.stockQuantity : 0;

    movements.forEach(m => {
      if (m.productId === productId && m.warehouseId === warehouseId) {
        stock += m.quantity;
      }
    });

    return stock;
  };

  const handleTransfer = async (e) => {
    e.preventDefault();

    const availableStock = getStockInWarehouse(Number(selectedProduct), Number(fromWarehouse));
    
    if (Number(quantity) > availableStock) {
      alert(`Operație imposibilă! În hală există doar ${availableStock} bucăți disponibile. Te rugăm să introduci o cantitate mai mică.`);
      return; 
    }

    if (fromWarehouse === toWarehouse) {
      alert("Hala de origine nu poate fi aceeași cu hala de destinație!");
      return;
    }

    try {
      await axios.post(`${API_BASE_URL}/Movements/transfer`, null, {
        params: {
          productId: selectedProduct,
          fromWarehouseId: fromWarehouse,
          toWarehouseId: toWarehouse,
          quantity: quantity
        }
      });
      alert("Transfer realizat cu succes!");
      loadData(); 
    } catch (error) {
      alert("Eroare la transfer: " + (error.response?.data || error.message));
    }
  };

  // Taiem lista la primele 3 elemente dacă tabelul nu e extins
  const displayedMovements = isTableExpanded ? movements : movements.slice(0, 3);

  return (
    <div className="app-container">
      <h1>Inventory App</h1>
      <hr />

      {/* 1. SECȚIUNEA HALE FILTRATE */}
      <h2>Status Hale / Depozite </h2>
      <div className="warehouses-wrapper">
        {warehouses.map(w => (
          <div key={w.id} className="warehouse-card">
            <h3>🏢 {w.name}</h3>
            <p className="location-text">📍 {w.location}</p>
            <h4>Produse în hală:</h4>
            <ul>
              {products.map(p => {
                const currentStock = getStockInWarehouse(p.id, w.id);
                return (
                  <li key={p.id} style={{ marginBottom: '5px' }}>
                    {p.name}: {currentStock > 0 ? (
                      <strong className="stock-good">{currentStock} buc.</strong>
                    ) : (
                      <span className="stock-empty">Lipsă </span>
                    )}
                  </li>
                );
              })}
            </ul>
          </div>
        ))}
      </div>

      {/* 2. FORMULAR TRANSFER */}
      <h2>Efectuează Transfer Între Hale</h2>
      <form onSubmit={handleTransfer} className="transfer-form">
        <div className="form-group">
          <label>Produs: </label>
          <select className="form-input" onChange={e => setSelectedProduct(e.target.value)} required>
            <option value="">Selectează produs...</option>
            {products.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label>Din Hala: </label>
          <select className="form-input" onChange={e => setFromWarehouse(e.target.value)} required>
            <option value="">Alege sursa...</option>
            {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label>În Hala: </label>
          <select className="form-input" onChange={e => setToWarehouse(e.target.value)} required>
            <option value="">Alege destinația...</option>
            {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label>Cantitate: </label>
          <input className="form-input-small" type="number" onChange={e => setQuantity(e.target.value)} min="1" required />
        </div>

        <button type="submit" className="submit-btn">
          Trimite Mișcare Stoc
        </button>
      </form>

      {/* 3. TABEL ISTORIC MIȘCĂRI */}
      <h2>Istoric Mișcări Stoc </h2>
      <table className="history-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Produs ID</th>
            <th>Hala ID</th>
            <th>Cantitate</th>
            <th>Tip Mișcare</th>
            <th>Dată / Oră</th>
          </tr>
        </thead>
        <tbody>
          {displayedMovements.map(m => (
            <tr key={m.id}>
              <td>{m.id}</td>
              <td>#{m.productId}</td>
              <td>#{m.warehouseId}</td>
              <td className={m.quantity > 0 ? "qty-positive" : "qty-negative"}>
                {m.quantity > 0 ? `+${m.quantity}` : m.quantity}
              </td>
              <td>{m.type}</td>
              <td className="date-text">{new Date(m.date).toLocaleString()}</td>
            </tr>
          ))}
          {movements.length === 0 && (
            <tr>
              <td colSpan="6" style={{ padding: '15px', textAlign: 'center', color: '#aaa' }}>Nu există nicio mișcare înregistrată.</td>
            </tr>
          )}
        </tbody>
      </table>

      {/* BUTON TOGGLE PENTRU TABEL */}
      {movements.length > 3 && (
        <button 
          className="toggle-btn" 
          onClick={() => setIsTableExpanded(!isTableExpanded)}
        >
          {isTableExpanded ? '▲ Mai putine' : '▼ Mai multe'}
        </button>
      )}
    </div>
  );
}

export default App;