from flask import Flask, request, jsonify
# from waitress import serve
import sqlite3


app = Flask(__name__)


def connect_db(db_name):
    return sqlite3.connect(f'{db_name}.db')


@app.before_request
def log_request_info():
    print(f"Received {request.method} request for {request.url}")


@app.route("/")
def default_route():
    return "<h1 style='color:blue'>ISOMET Dual Tracking System Server</h1> <p>This website is the backend for the ISOMET 'DTS' system.</p>"

@app.route('/<db_name>/<table_name>/add', methods=['POST'])
def add_entry(db_name, table_name):
    try:
        data = request.json
        print(f"Received data: {data}")
        columns = ', '.join(data.keys())
        placeholders = ', '.join(['?' for _ in data.values()])
        values = tuple(data.values())
        print(f"SQL Query: INSERT INTO {table_name} ({columns}) VALUES ({placeholders})")
        print(f"Values: {values}")

        conn = connect_db(db_name)
        cursor = conn.cursor()
        cursor.execute(f"INSERT INTO {table_name} ({columns}) VALUES ({placeholders})", values)
        conn.commit()
        conn.close()
        print("Database modified successfully.")
        return jsonify({'status': 'success'})
    except sqlite3.Error as e:
        print(f"SQLite error: {e}")
        return jsonify({'status': 'error', 'message': str(e)})
    except Exception as e:
        print(f"General error: {e}")
        return jsonify({'status': 'error', 'message': str(e)})


@app.route('/<db_name>/<table_name>/get', methods=['GET'])
def get_entries(db_name, table_name):
    try:
        barcode = request.args.get('barcode')
        report = request.args.get('report', 'false').lower() == 'true'
        conn = connect_db(db_name)
        cursor = conn.cursor()
        if barcode:
            if report:
                cursor.execute(f"SELECT * FROM {table_name} WHERE barcode = ? ORDER BY date DESC LIMIT 100", (barcode,))
            else:
                cursor.execute(f"SELECT * FROM {table_name} WHERE barcode = ? ORDER BY date DESC LIMIT 1", (barcode,))
        else:
            cursor.execute(f"SELECT * FROM {table_name}")
        rows = cursor.fetchall()
        conn.close()
        entries = [dict(zip([column[0] for column in cursor.description], row)) for row in rows]
        print(f"Entries: {entries}")
        return jsonify(entries)
    except sqlite3.Error as e:
        print(f"SQLite error: {e}")
        return jsonify({'status': 'error', 'message': str(e)})
    except Exception as e:
        print(f"General error: {e}")
        return jsonify({'status': 'error', 'message': str(e)})

@app.route('/<db_name>/<table_name>/getrange', methods=['GET'])
def get_entry_range(db_name, table_name):
    try:
        start_range = request.args.get('startRange', type=int)
        end_range = request.args.get('endRange', type=int)
        conn = connect_db(db_name)
        cursor = conn.cursor()
        if start_range is None or end_range is None:
            return jsonify({'status': 'error', 'message': 'Missing range parameters'}), 400
        sql_query = f"""
            SELECT * FROM {table_name}
            WHERE CAST(barcode AS INTEGER) BETWEEN ? AND ?
            AND date = (SELECT MAX(date) FROM {table_name} AS sub WHERE sub.barcode = {table_name}.barcode)
            ORDER BY barcode ASC
        """
        cursor.execute(sql_query, (start_range, end_range,))
        rows = cursor.fetchall()
        conn.close()
        entries = [dict(zip([column[0] for column in cursor.description], row)) for row in rows]
        print(f"Entries: {entries}")
        return jsonify(entries)
    except sqlite3.Error as e:
        print(f"SQLite error: {e}")
        return jsonify({'status': 'error', 'message': str(e)})
    except Exception as e:
        print(f"General error: {e}")
        return jsonify({'status': 'error', 'message': str(e)})

if __name__ == '__main__':
    #context = ('cert.pem', 'key.pem')
    #serve(app, host="0.0.0.0", port=5000, url_scheme="https")
    app.run(debug=False, host="0.0.0.0")
