import { Card } from '../../ui/components/Card'

function TestCard() {
    return (
        <>
            <Card className="mb-2 me-2" style={{ maxWidth: '500px' }}>
                <div className="card-header">User Information</div>
                <div className="card-body">
                    <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                        Integer hendrerit, turpis ut ullamcorper eleifend, ex
                        enim lobortis est, eget faucibus enim dolor eu ex.
                    </p>
                </div>
            </Card>
        </>
    )
}

export { TestCard }
