from flask import Flask, jsonify, request
import swype

app = Flask(__name__)

@app.route('/suggest', methods=['GET'])
def get_tasks():
    query = request.args.get('query', None)
    print query;
    if query is not None and len(query) > 0:
        suggestions = swype.get_suggestion(query)
        return jsonify({'query':query, 'suggestions':suggestions, 'error':'None'})
    else:
         return jsonify({'error':'No query argument given!', suggestions:[],query:None})

if __name__ == '__main__':
    app.run(debug=True)
