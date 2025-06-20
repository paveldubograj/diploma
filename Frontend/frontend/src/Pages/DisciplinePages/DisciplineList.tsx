import React, { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Alert, Spinner, Container, Row, Col } from 'react-bootstrap';
import { fetchDisciplineById, UpdateDiscipline, CreateDiscipline, DeleteDiscipline, fetchDisciplinesAdmin } from '../../api/disciplineApi';
import { Discipline } from '../../types'

const DisciplinesPage: React.FC = () => {
    const [disciplines, setDisciplines] = useState<Discipline[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [showCreateModal, setShowCreateModal] = useState<boolean>(false);
    const [showEditModal, setShowEditModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [currentDiscipline, setCurrentDiscipline] = useState<Discipline | null>(null);
    const [newDiscipline, setNewDiscipline] = useState<Pick<Discipline, 'name' | 'description'>>({
        name: '',
        description: ''
    });

    useEffect(() => {
        loadDisciplines();
    }, []);

    const loadDisciplines = async () => {
        try {
            setLoading(true);
            const data = await fetchDisciplinesAdmin();
            setDisciplines(data);
            setError(null);
        } catch (err) {
            setError('Failed to load disciplines');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async () => {
        try {
            if (newDiscipline !== null) {
                setLoading(true);
                const createdDiscipline = await CreateDiscipline(newDiscipline);
                if (createdDiscipline) {
                    setDisciplines([...disciplines, createdDiscipline]);
                    setShowCreateModal(false);
                    setNewDiscipline({
                        name: '',
                        description: ''
                    });
                }
            }
        } catch (err) {
            setError('Failed to create discipline');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleUpdate = async () => {
        if (!currentDiscipline) return;

        try {
            setLoading(true);
            const updatedDiscipline = await UpdateDiscipline(currentDiscipline);
            if (updatedDiscipline) {
                setDisciplines(disciplines.map(d =>
                    d.id === updatedDiscipline.id ? updatedDiscipline : d
                ));
                setShowEditModal(false);
            }
        } catch (err) {
            setError('Failed to update discipline');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async () => {
        if (!currentDiscipline) return;

        try {
            setLoading(true);
            const deletedDiscipline = await DeleteDiscipline(currentDiscipline.id);
            if (deletedDiscipline) {
                setDisciplines(disciplines.filter(d => d.id !== currentDiscipline.id));
                setShowDeleteModal(false);
            }
        } catch (err) {
            setError('Failed to delete discipline');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const openEditModal = async (id: string) => {
        try {
            setLoading(true);
            const discipline = await fetchDisciplineById(id);
            setCurrentDiscipline(discipline);
            setShowEditModal(true);
        } catch (err) {
            setError('Failed to load discipline');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const openDeleteModal = (discipline: Discipline) => {
        setCurrentDiscipline(discipline);
        setShowDeleteModal(true);
    };

    return (
        <Container className="mt-4">
            <Row className="mb-4">
                <Col>
                    <h1>Disciplines Management</h1>
                </Col>
                <Col className="text-end">
                    <Button variant="primary" onClick={() => setShowCreateModal(true)}>
                        Add New Discipline
                    </Button>
                </Col>
            </Row>

            {error && <Alert variant="danger">{error}</Alert>}

            {loading ? (
                <div className="text-center">
                    <Spinner animation="border" />
                </div>
            ) : (
                <Table striped bordered hover responsive>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Created At</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {disciplines.map(discipline => (
                            <tr key={discipline.id}>
                                <td>{discipline.name}</td>
                                <td>{discipline.description}</td>
                                <td>{new Date(discipline.createdAt).toLocaleString()}</td>
                                <td>
                                    <Button
                                        variant="info"
                                        size="sm"
                                        onClick={() => openEditModal(discipline.id)}
                                        className="me-2"
                                    >
                                        Edit
                                    </Button>
                                    <Button
                                        variant="danger"
                                        size="sm"
                                        onClick={() => openDeleteModal(discipline)}
                                    >
                                        Delete
                                    </Button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            )}

            {/* Create Discipline Modal */}
            <Modal show={showCreateModal} onHide={() => setShowCreateModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Create New Discipline</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Name</Form.Label>
                            <Form.Control
                                type="text"
                                value={newDiscipline.name}
                                onChange={(e) => setNewDiscipline({ ...newDiscipline, name: e.target.value })}
                            />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Description</Form.Label>
                            <Form.Control
                                as="textarea"
                                rows={3}
                                value={newDiscipline.description}
                                onChange={(e) => setNewDiscipline({ ...newDiscipline, description: e.target.value })}
                            />
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowCreateModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleCreate} disabled={loading}>
                        {loading ? <Spinner size="sm" /> : 'Create'}
                    </Button>
                </Modal.Footer>
            </Modal>

            {/* Edit Discipline Modal */}
            <Modal show={showEditModal} onHide={() => setShowEditModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Edit Discipline</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {currentDiscipline && (
                        <Form>
                            <Form.Group className="mb-3">
                                <Form.Label>Name</Form.Label>
                                <Form.Control
                                    type="text"
                                    value={currentDiscipline.name}
                                    onChange={(e) => setCurrentDiscipline({ ...currentDiscipline, name: e.target.value })}
                                />
                            </Form.Group>
                            <Form.Group className="mb-3">
                                <Form.Label>Description</Form.Label>
                                <Form.Control
                                    as="textarea"
                                    rows={3}
                                    value={currentDiscipline.description}
                                    onChange={(e) => setCurrentDiscipline({ ...currentDiscipline, description: e.target.value })}
                                />
                            </Form.Group>
                        </Form>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowEditModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleUpdate} disabled={loading}>
                        {loading ? <Spinner size="sm" /> : 'Save Changes'}
                    </Button>
                </Modal.Footer>
            </Modal>

            {/* Delete Confirmation Modal */}
            <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm Delete</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Are you sure you want to delete the discipline "{currentDiscipline?.name}"?
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="danger" onClick={handleDelete} disabled={loading}>
                        {loading ? <Spinner size="sm" /> : 'Delete'}
                    </Button>
                </Modal.Footer>
            </Modal>
        </Container>
    );
};

export default DisciplinesPage;
